extends Area2D
## Wave enemy — ejected from an edge emitter, then patrols in formation ALONG
## that edge (a bobbing curtain that slides side to side and bounces at the
## bounds), the way the Unity EnemyWave curtain behaves. It does not home the
## player; it's a moving wall you weave through. Own art: 2-frame neon triangle.

@export var patrol_speed: float = 240.0
@export var bob_amplitude: float = 70.0
@export var bob_frequency: float = 2.0
@export var settle_depth: float = 150.0     # how far inward it settles from the edge
@export var max_health: float = 60.0
@export var points: int = 10
@export var bounds: Vector2 = Vector2(520, 950)

var _health: float
var _time: float = 0.0
var _inward: Vector2 = Vector2.DOWN         # toward the arena centre
var _patrol_dir: Vector2 = Vector2.RIGHT    # along the edge
var _base: Vector2                          # settle-line anchor (bob pivots around this)
var _launch_vel: Vector2 = Vector2.ZERO
var _settle_t: float = 0.6                   # ejection/settle phase timer

func spawn_launch(vel: Vector2, index: int, _count: int, eject_dir: Vector2) -> void:
	_launch_vel = vel
	_inward = eject_dir.normalized()
	_patrol_dir = _inward.orthogonal()
	# alternate initial slide direction is shared across the row via a coin flip
	if (index % 2) == 1:
		_patrol_dir = -_patrol_dir

func _ready() -> void:
	_health = max_health
	_time = randf_range(0.0, TAU)
	add_to_group("enemies")
	var sprite := $Sprite as AnimatedSprite2D
	sprite.sprite_frames = SheetAnim.build_from_textures([
		load("res://art/enemy_wave_1.png"), load("res://art/enemy_wave_2.png")
	], 5.0)
	sprite.frame = randi() % 2
	sprite.play("default")
	# settle line = current pos pushed inward by settle_depth
	_base = global_position + _inward * settle_depth

func _physics_process(delta: float) -> void:
	_time += delta
	if _settle_t > 0.0:
		# ejection: fly out from the pod, easing into the settle line
		_settle_t -= delta
		global_position += _launch_vel * delta
		_launch_vel = _launch_vel.move_toward(Vector2.ZERO, 900.0 * delta)
		global_position = global_position.move_toward(_base, 260.0 * delta)
		return
	# patrol along the edge, bounce at bounds
	_base += _patrol_dir * patrol_speed * delta
	if absf(_base.x) > bounds.x:
		_base.x = clampf(_base.x, -bounds.x, bounds.x)
		_patrol_dir.x = -_patrol_dir.x
	if absf(_base.y) > bounds.y:
		_base.y = clampf(_base.y, -bounds.y, bounds.y)
		_patrol_dir.y = -_patrol_dir.y
	# bob in/out perpendicular to the patrol direction
	var bob := _inward * sin(_time * bob_frequency) * bob_amplitude
	global_position = _base + bob

func take_damage(amount: float) -> void:
	_health -= amount
	modulate = Color(2.5, 2.5, 2.5)
	if _health <= 0.0:
		_die()
	else:
		var t := create_tween()
		t.tween_property(self, "modulate", Color.WHITE, 0.12)

func _die() -> void:
	GameSession.add_points(points)
	var grid := get_tree().get_first_node_in_group("spring_grid")
	if grid != null and grid.has_method("disturb"):
		grid.disturb(global_position, 700.0)
	Main.spawn_death_vfx(global_position, Color(1.0, 0.4, 1.4), true)
	Main.spawn_score_popup(global_position, points * GameSession.multiplier, Color(1.0, 0.5, 1.6))
	Main.spawn_mult_bits(global_position, 2, 5)
	Settings.buzz(18)
	queue_free()
