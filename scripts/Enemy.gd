extends Area2D
## Homing enemy — accelerating chase, mirrors EnemyFollowPlayer.cs + Enemy.cs.

@export var homing_speed_min: float = 260.0
@export var homing_speed_max: float = 340.0
@export var acceleration: float = 380.0
@export var max_health: float = 100.0
@export var points: int = 5

var _health: float
var _actual_speed: float = 0.0
var _rand_speed: float = 0.0

func _ready() -> void:
	_health = max_health
	_rand_speed = randf_range(homing_speed_min, homing_speed_max)
	add_to_group("enemies")
	var sprite := $Sprite as AnimatedSprite2D
	sprite.sprite_frames = SheetAnim.build(load("res://art/enemy_green_sheet.png"), 36, 20.0)
	sprite.frame = randi() % 48  # desync so the swarm doesn't pulse in lockstep
	sprite.play("default")

func _physics_process(delta: float) -> void:
	var player := Player.instance
	if player == null:
		return
	if _actual_speed < _rand_speed:
		_actual_speed += acceleration * delta
	global_position = global_position.move_toward(player.global_position, _actual_speed * delta)

func take_damage(amount: float) -> void:
	_health -= amount
	modulate = Color(2, 2, 2)  # flash
	if _health <= 0.0:
		_die()
	else:
		var t := create_tween()
		t.tween_property(self, "modulate", Color.WHITE, 0.12)

func _die() -> void:
	GameSession.add_points(points)
	GameSession.bump_multiplier()
	var grid := get_tree().get_first_node_in_group("spring_grid")
	if grid != null and grid.has_method("disturb"):
		grid.disturb(global_position, 900.0)
	Main.spawn_explosion(global_position)
	queue_free()
