extends Node2D
## Wave spawner — spawns mixed enemy types on a shrinking interval.
## basic-only early; wave enemies join at 20s; smart enemies at 60s.

@export var enemy_scene: PackedScene
@export var enemy_wave_scene: PackedScene
@export var enemy_smart_scene: PackedScene
@export var spawn_interval: float = 1.4
@export var min_interval: float = 0.32
@export var ramp_per_wave: float = 0.035
@export var edge: Vector2 = Vector2(560, 980)

var _timer: float = 0.0
var _interval: float
var _elapsed: float = 0.0

func _ready() -> void:
	_interval = spawn_interval

func _physics_process(delta: float) -> void:
	if not GameSession.is_running:
		return
	_elapsed += delta
	_timer -= delta
	if _timer <= 0.0:
		_timer = _interval
		_interval = maxf(min_interval, _interval - ramp_per_wave)
		_spawn()

func _spawn() -> void:
	var scene := _pick_scene()
	if scene == null:
		return
	var e := scene.instantiate()
	get_parent().add_child(e)
	var pos := Vector2.ZERO
	if randf() < 0.5:
		pos.x = (-edge.x if randf() < 0.5 else edge.x)
		pos.y = randf_range(-edge.y, edge.y)
	else:
		pos.x = randf_range(-edge.x, edge.x)
		pos.y = (-edge.y if randf() < 0.5 else edge.y)
	e.global_position = pos

func _pick_scene() -> PackedScene:
	var wave_w := clampf((_elapsed - 20.0) / 40.0, 0.0, 0.35)
	var smart_w := clampf((_elapsed - 60.0) / 60.0, 0.0, 0.25)
	var r := randf()
	if r < smart_w and enemy_smart_scene != null:
		return enemy_smart_scene
	if r < smart_w + wave_w and enemy_wave_scene != null:
		return enemy_wave_scene
	return enemy_scene
