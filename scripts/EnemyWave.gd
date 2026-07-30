extends Area2D
## Wave enemy — sinusoidal approach, weaves side to side.
## Harder to predict than straight-line chasers; tinted cyan.

@export var approach_speed: float = 220.0
@export var weave_amplitude: float = 180.0
@export var weave_frequency: float = 2.4
@export var max_health: float = 60.0
@export var points: int = 10

var _health: float
var _time: float = 0.0
var _perp: Vector2 = Vector2.ZERO
var _init_pos: Vector2

func _ready() -> void:
	_health = max_health
	_time = randf_range(0.0, TAU)
	_init_pos = global_position
	add_to_group("enemies")
	var sprite := $Sprite as AnimatedSprite2D
	# Own art: neon-triangle "wave" enemy (2-frame pulse).
	sprite.sprite_frames = SheetAnim.build_from_textures([
		load("res://art/enemy_wave_1.png"), load("res://art/enemy_wave_2.png")
	], 5.0)
	sprite.frame = randi() % 2
	sprite.play("default")

func _physics_process(delta: float) -> void:
	var player := Player.instance
	if player == null:
		return
	_time += delta * weave_frequency
	var to_player := (player.global_position - global_position)
	var fwd := to_player.normalized()
	_perp = Vector2(-fwd.y, fwd.x)
	var weave := _perp * sin(_time) * weave_amplitude
	global_position += (fwd * approach_speed + weave) * delta

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
	Main.spawn_explosion(global_position)
	Main.spawn_score_popup(global_position, points * GameSession.multiplier, Color(0.6, 2.0, 1.8))
	Main.spawn_mult_bits(global_position, 2, 5)
	Input.vibrate_handheld(18)
	queue_free()
