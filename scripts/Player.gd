extends Area2D
class_name Player
## One-finger drag-to-move + auto-fire. Singleton, mirrors Player.cs.
## Touch/drag anywhere moves the ship toward the finger; firing is automatic
## at the nearest enemy (the original's "just one finger" feel).

signal died

static var instance: Player

@export var move_speed: float = 900.0
@export var fire_period: float = 0.28
@export var projectile_scene: PackedScene
@export var playfield_half: Vector2 = Vector2(900, 1650)
## Floating-joystick radius (px, in viewport space): full-tilt speed is reached
## when the finger is this far from where the touch began.
@export var joystick_radius: float = 220.0

var _joy_origin: Vector2       # where the touch/press began (viewport space)
var _joy_vec: Vector2          # current stick vector, magnitude 0..1
var _dragging: bool = false
var _fire_cooldown: float = 0.0
var _alive: bool = true
var _shot_count: int = 1
var _cur_fire_period: float
var _drone_count: int = 0
var _drone_ring: Node2D
var _projectile_damage: float = 50.0
var _projectile_pierce: int = 0
var _trail: PlayerTrail

func _ready() -> void:
	instance = self
	_joy_vec = Vector2.ZERO
	_cur_fire_period = fire_period
	add_to_group("player")
	var sprite := $Sprite as AnimatedSprite2D
	sprite.sprite_frames = SheetAnim.build(load("res://art/player_sheet.png"), 36, 18.0)
	sprite.play("default")
	_drone_ring = DroneRing.new()
	add_child(_drone_ring)
	_trail = PlayerTrail.new()
	_trail.z_index = -2
	add_child(_trail)
	_trail.track(global_position, 0.0)

func apply_upgrade(kind: String) -> void:
	match kind:
		"drone":
			_drone_count = min(_drone_count + 1, 4)
			_drone_ring.count = _drone_count
		"spread":
			_shot_count = min(_shot_count + 1, 5)
		"overcharge":
			_cur_fire_period = maxf(0.10, _cur_fire_period * 0.84)
		"drone_swarm":
			_drone_count = min(_drone_count + 2, 6)
			_drone_ring.count = _drone_count
		"pierce":
			_projectile_pierce = min(_projectile_pierce + 1, 4)
		"amplify":
			_projectile_damage = min(_projectile_damage + 18.0, 140.0)

## Floating virtual joystick: wherever a touch begins becomes the stick centre;
## the offset of the finger from that centre (clamped to joystick_radius) drives
## movement direction + speed. Lifting the finger stops the ship.
func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if event.pressed:
			_begin_stick(event.position)
		else:
			_end_stick()
	elif event is InputEventScreenDrag:
		_update_stick(event.position)
	elif event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		if event.pressed:
			_begin_stick(event.position)
		else:
			_end_stick()
	elif event is InputEventMouseMotion and _dragging:
		_update_stick(event.position)

func _begin_stick(pos: Vector2) -> void:
	_dragging = true
	_joy_origin = pos
	_joy_vec = Vector2.ZERO

func _update_stick(pos: Vector2) -> void:
	if not _dragging:
		return
	var offset := pos - _joy_origin
	if offset.length() > joystick_radius:
		offset = offset.normalized() * joystick_radius
	_joy_vec = offset / joystick_radius   # magnitude 0..1

func _end_stick() -> void:
	_dragging = false
	_joy_vec = Vector2.ZERO

func _physics_process(delta: float) -> void:
	if not _alive:
		return
	if Main.demo_mode or OS.has_environment("PIU_DEMO"):
		_autopilot(delta)
	var prev := global_position
	global_position += _joy_vec * move_speed * delta
	global_position.x = clampf(global_position.x, -playfield_half.x, playfield_half.x)
	global_position.y = clampf(global_position.y, -playfield_half.y, playfield_half.y)
	var moved := global_position.distance_to(prev)
	if moved > 0.5:
		var grid := get_tree().get_first_node_in_group("spring_grid")
		if grid != null and grid.has_method("disturb"):
			grid.disturb(global_position, moved * Settings.player_distortion, clampf(45.0 + Settings.player_distortion * 2.2, 55.0, 125.0))
	_trail.track(global_position, delta)

	_fire_cooldown -= delta
	if _fire_cooldown <= 0.0:
		_fire_cooldown = _cur_fire_period
		_fire()
	queue_redraw()   # targeting line

## Faint targeting line to the nearest enemy (Unity DrawLineToClosestEnemy).
func _draw() -> void:
	if not _alive:
		return
	var e := _nearest_enemy()
	if e == null:
		return
	draw_line(Vector2.ZERO, to_local(e.global_position), Color(0.5, 1.1, 2.0, 0.28), 2.0, true)

## Demo autopilot for the title-screen background: drift on a lissajous path
## while steering away from the nearest enemy. Just needs to look alive.
var _demo_t: float = 0.0
func _autopilot(delta: float) -> void:
	_demo_t += delta
	var wander := Vector2(sin(_demo_t * 0.7) * 360.0, cos(_demo_t * 0.9) * 620.0)
	var flee := Vector2.ZERO
	var e := _nearest_enemy()
	if e != null:
		var away := global_position - e.global_position
		if away.length() < 320.0:
			flee = away.normalized() * (320.0 - away.length()) * 3.0
	var target := wander + flee
	_joy_vec = (target - global_position).limit_length(joystick_radius) / joystick_radius

func _fire() -> void:
	if projectile_scene == null:
		return
	var enemy := _nearest_enemy()
	var base_dir := Vector2.UP
	if enemy != null:
		base_dir = (enemy.global_position - global_position).normalized()
	var spread_step := 0.18
	var half := (_shot_count - 1) * 0.5
	for i in _shot_count:
		var angle := (i - half) * spread_step
		var dir := base_dir.rotated(angle)
		var shot := projectile_scene.instantiate()
		get_parent().add_child(shot)
		shot.global_position = global_position
		shot.setup(dir, _projectile_damage, _projectile_pierce)
	# Orbit drones visibly escort the ship and fire independently from their
	# current positions. One upgrade therefore changes both the silhouette and
	# the combat pattern.
	for i in _drone_count:
		var drone_pos: Vector2 = _drone_ring.drone_position(i)
		var drone_dir: Vector2 = base_dir
		if enemy != null:
			drone_dir = (enemy.global_position - drone_pos).normalized()
		var drone_shot := projectile_scene.instantiate()
		get_parent().add_child(drone_shot)
		drone_shot.global_position = drone_pos
		drone_shot.setup(drone_dir, _projectile_damage * 0.80, _projectile_pierce)

func _nearest_enemy() -> Node2D:
	var best: Node2D = null
	var best_d := INF
	for e in get_tree().get_nodes_in_group("enemies"):
		var d: float = global_position.distance_squared_to(e.global_position)
		if d < best_d:
			best_d = d
			best = e
	return best

func _on_body_entered(_b: Node) -> void:
	hit()

func _on_area_entered(a: Area2D) -> void:
	if a.is_in_group("enemies"):
		hit()

func hit() -> void:
	if not _alive:
		return
	if Main.demo_mode or OS.has_environment("PIU_DEMO"):
		# demo/recording never dies — nudge to centre and shrug it off
		global_position = Vector2.ZERO
		return
	if Settings.god_mode:
		return   # invulnerable for testing mechanics
	_alive = false
	Settings.buzz(150)
	died.emit()
	GameSession.end_run()

class DroneRing extends Node2D:
	var count: int = 0:
		set(value):
			count = value
			queue_redraw()
	var phase: float = 0.0

	func _process(delta: float) -> void:
		phase += delta * 2.4
		queue_redraw()

	func drone_position(index: int) -> Vector2:
		if count <= 0:
			return global_position
		return global_position + Vector2.from_angle(phase + TAU * index / count) * 105.0

	func _draw() -> void:
		for i in count:
			var p := Vector2.from_angle(phase + TAU * i / count) * 105.0
			draw_circle(p, 17.0, Color(1.4, 0.45, 2.0, 0.24))
			draw_circle(p, 9.0, Color(1.7, 0.65, 2.3, 1.0))
			draw_arc(p, 19.0, phase, phase + PI * 1.4, 16, Color(0.5, 1.4, 2.2, 0.9), 2.5)

## Long, tapered HDR ribbon made from recent player positions. The bright head
## and soft magenta/cyan wake are caught by the WorldEnvironment bloom.
class PlayerTrail extends Node2D:
	const MAX_AGE := 1.45
	const SAMPLE_TIME := 0.035
	const MAX_POINTS := 54
	var _points: Array[Vector2] = []
	var _ages: Array[float] = []
	var _sample_timer := 0.0

	func track(world_pos: Vector2, delta: float) -> void:
		for i in _ages.size():
			_ages[i] += delta
		_sample_timer += delta
		var needs_sample := _points.is_empty() or _sample_timer >= SAMPLE_TIME
		if needs_sample:
			_points.push_front(to_local(world_pos))
			_ages.push_front(0.0)
			_sample_timer = 0.0
		while _points.size() > MAX_POINTS or (not _ages.is_empty() and _ages[-1] > MAX_AGE):
			_points.pop_back()
			_ages.pop_back()
		queue_redraw()

	func _draw() -> void:
		if _points.size() < 2:
			return
		for i in _points.size() - 1:
			var t := clampf(_ages[i] / MAX_AGE, 0.0, 1.0)
			var alpha := pow(1.0 - t, 1.55)
			var width := lerpf(26.0, 1.5, t)
			var color := Color(0.32 + t * 0.9, 1.25 - t * 0.55, 2.0, alpha * 0.74)
			# Wide translucent pass becomes the soft neon halo; the narrow core
			# keeps the wake clean and fast rather than smoky.
			draw_line(_points[i], _points[i + 1], Color(color.r, color.g, color.b, alpha * 0.16), width * 2.7, true)
			draw_line(_points[i], _points[i + 1], color, width, true)
		if _ages[0] < 0.16:
			var head_alpha := 1.0 - _ages[0] / 0.16
			draw_circle(_points[0], 22.0 * head_alpha, Color(0.45, 1.4, 2.4, head_alpha * 0.2))
