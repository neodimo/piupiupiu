extends Area2D
## Smart enemy — orbits and dashes at the player with brief dodge sidesteps.
## Uses enemy_green_sheet.png tinted purple to visually distinguish.

@export var orbit_speed: float = 190.0
@export var dash_speed: float = 680.0
@export var orbit_radius: float = 380.0
@export var dash_interval: float = 2.2
@export var dodge_chance: float = 0.35
@export var max_health: float = 200.0
@export var points: int = 20

var _health: float
var _dash_timer: float = 0.0
var _dashing: bool = false
var _dash_dir: Vector2 = Vector2.ZERO
var _dash_time: float = 0.0
var _orbit_angle: float = 0.0
var _launch_vel: Vector2 = Vector2.ZERO

func spawn_launch(vel: Vector2, _index: int, _count: int, _eject_dir: Vector2) -> void:
	_launch_vel = vel

func _ready() -> void:
	_health = max_health
	_orbit_angle = randf_range(0.0, TAU)
	_dash_timer = randf_range(0.8, dash_interval)
	add_to_group("enemies")
	var sprite := $Sprite as AnimatedSprite2D
	# Own art: cyan ring / pink-core "smart" enemy (static).
	sprite.sprite_frames = SheetAnim.build_from_textures([load("res://art/enemy_smart.png")])
	sprite.play("default")

func _physics_process(delta: float) -> void:
	if _launch_vel.length() > 8.0:
		global_position += _launch_vel * delta
		_launch_vel = _launch_vel.move_toward(Vector2.ZERO, 1100.0 * delta)
	var player := Player.instance
	if player == null:
		return
	if _dashing:
		global_position += _dash_dir * dash_speed * delta
		_dash_time -= delta
		if _dash_time <= 0.0:
			_dashing = false
		return
	_dash_timer -= delta
	if _dash_timer <= 0.0:
		_dash_timer = dash_interval
		if randf() < dodge_chance:
			_dash_dir = Vector2(randf_range(-1, 1), randf_range(-1, 1)).normalized()
		else:
			_dash_dir = (player.global_position - global_position).normalized()
		_dashing = true
		_dash_time = 0.22
		return
	_orbit_angle += orbit_speed / orbit_radius * delta
	var target := player.global_position + Vector2(cos(_orbit_angle), sin(_orbit_angle)) * orbit_radius
	global_position = global_position.move_toward(target, orbit_speed * delta)

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
		grid.disturb(global_position, 1200.0)
	Main.spawn_death_vfx(global_position, Color(0.5, 1.6, 2.2))
	Main.spawn_score_popup(global_position, points * GameSession.multiplier, Color(0.6, 1.4, 2.4))
	Main.spawn_mult_bits(global_position, 3, 7)
	Settings.buzz(30)
	queue_free()
