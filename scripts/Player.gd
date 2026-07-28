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
@export var playfield_half: Vector2 = Vector2(520, 940)

var _target: Vector2
var _dragging: bool = false
var _fire_cooldown: float = 0.0
var _alive: bool = true

func _ready() -> void:
	instance = self
	_target = global_position
	add_to_group("player")
	var sprite := $Sprite as AnimatedSprite2D
	sprite.sprite_frames = SheetAnim.build(load("res://art/player_sheet.png"), 36, 18.0)
	sprite.play("default")

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		_dragging = event.pressed
		if event.pressed:
			_target = event.position - get_viewport_rect().size * 0.5
	elif event is InputEventScreenDrag:
		_target = event.position - get_viewport_rect().size * 0.5
	elif event is InputEventMouseButton:
		_dragging = event.pressed
		if event.pressed:
			_target = get_global_mouse_position()
	elif event is InputEventMouseMotion and _dragging:
		_target = get_global_mouse_position()

func _physics_process(delta: float) -> void:
	if not _alive:
		return
	global_position = global_position.move_toward(_target, move_speed * delta)
	global_position.x = clampf(global_position.x, -playfield_half.x, playfield_half.x)
	global_position.y = clampf(global_position.y, -playfield_half.y, playfield_half.y)

	_fire_cooldown -= delta
	if _fire_cooldown <= 0.0:
		_fire_cooldown = fire_period
		_fire()

func _fire() -> void:
	if projectile_scene == null:
		return
	var enemy := _nearest_enemy()
	var dir := Vector2.UP
	if enemy != null:
		dir = (enemy.global_position - global_position).normalized()
	var shot := projectile_scene.instantiate()
	get_parent().add_child(shot)
	shot.global_position = global_position
	shot.setup(dir)

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
	_alive = false
	died.emit()
	GameSession.end_run()
