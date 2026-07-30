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
	sprite.sprite_frames = SheetAnim.build(load("res://art/enemy_green_sheet.png"), 36, 20.0)
	sprite.frame = randi() % 48
	sprite.play("default")
	modulate = Color(0.2, 1.0, 0.9)

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
	modulate = Color(2.0, 2.0, 2.0)
	if _health <= 0.0:
		_die()
	else:
		var t := create_tween()
		t.tween_property(self, "modulate", Color(0.2, 1.0, 0.9), 0.12)

func _die() -> void:
	GameSession.add_points(points)
	GameSession.bump_multiplier()
	var grid := get_tree().get_first_node_in_group("spring_grid")
	if grid != null and grid.has_method("disturb"):
		grid.disturb(global_position, 700.0)
	Main.spawn_explosion(global_position)
	Input.vibrate_handheld(18)
	queue_free()
