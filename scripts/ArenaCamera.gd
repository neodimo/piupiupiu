extends Camera2D
## Unity parity camera: follows the player at 0.8x so the arena moves around
## them, while camera limits keep the world/grid edges from entering the view.

@export var arena_half: Vector2 = Vector2(950.0, 1750.0)
@export var follow_factor: float = 0.8
@export var follow_lerp: float = 8.0

func _ready() -> void:
	position = Vector2.ZERO
	limit_left = int(-arena_half.x)
	limit_right = int(arena_half.x)
	limit_top = int(-arena_half.y)
	limit_bottom = int(arena_half.y)
	position_smoothing_enabled = false

func _process(delta: float) -> void:
	var player := Player.instance
	if player == null:
		return
	var target := player.global_position * follow_factor
	global_position = global_position.lerp(target, 1.0 - exp(-follow_lerp * delta))
