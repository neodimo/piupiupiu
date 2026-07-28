extends Node2D
## Wave spawner — spawns enemies at the arena edges on a shrinking interval.
## Mirrors EnemySpawner.cs / WaveConfig.cs pacing.

@export var enemy_scene: PackedScene
@export var spawn_interval: float = 1.4
@export var min_interval: float = 0.35
@export var ramp_per_wave: float = 0.04
@export var edge: Vector2 = Vector2(560, 980)

var _timer: float = 0.0
var _interval: float

func _ready() -> void:
	_interval = spawn_interval

func _physics_process(delta: float) -> void:
	if not GameSession.is_running:
		return
	_timer -= delta
	if _timer <= 0.0:
		_timer = _interval
		_interval = maxf(min_interval, _interval - ramp_per_wave)
		_spawn()

func _spawn() -> void:
	if enemy_scene == null:
		return
	var e := enemy_scene.instantiate()
	get_parent().add_child(e)
	# pick a random point on the arena border
	var pos := Vector2.ZERO
	if randf() < 0.5:
		pos.x = (-edge.x if randf() < 0.5 else edge.x)
		pos.y = randf_range(-edge.y, edge.y)
	else:
		pos.x = randf_range(-edge.x, edge.x)
		pos.y = (-edge.y if randf() < 0.5 else edge.y)
	e.global_position = pos
