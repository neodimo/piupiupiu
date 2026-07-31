extends Node2D
## Wave spawner — places emitter pods at the arena edges; each pod telegraphs
## then ejects its enemy(s) inward. basic-only early; wave enemies join at 20s
## (ejected as an edge formation); smart enemies at 60s.

@export var emitter_scene: PackedScene
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
	# pick an edge and the inward eject direction
	var pos := Vector2.ZERO
	var eject := Vector2.DOWN
	if randf() < 0.5:
		if randf() < 0.5:
			pos.x = -edge.x; eject = Vector2.RIGHT
		else:
			pos.x = edge.x; eject = Vector2.LEFT
		pos.y = randf_range(-edge.y * 0.7, edge.y * 0.7)
	else:
		if randf() < 0.5:
			pos.y = -edge.y; eject = Vector2.DOWN
		else:
			pos.y = edge.y; eject = Vector2.UP
		pos.x = randf_range(-edge.x * 0.7, edge.x * 0.7)

	if emitter_scene == null:
		# fallback: direct spawn (keeps game running if pod art missing)
		var e := scene.instantiate()
		get_parent().add_child(e)
		e.global_position = pos
		return

	# Configure BEFORE add_child so _ready() sees the final position + params
	# (otherwise the pod's spawn-disturb fires at world-origin, pinching centre).
	var pod := emitter_scene.instantiate() as Emitter
	pod.position = pos
	pod.enemy_scene = scene
	pod.eject_dir = eject
	if scene == enemy_wave_scene:
		pod.elongated = true              # only wave enemies use the long emitter
		pod.count = randi_range(4, 6)     # a curtain
		pod.eject_speed = 380.0
		pod.spread = 130.0
		pod.warn_time = 0.9
		pod.tint = Color(1.8, 0.9, 2.0)
	elif scene == enemy_smart_scene:
		pod.count = 1
		pod.eject_speed = 560.0
		pod.warn_time = 0.6
		pod.tint = Color(1.0, 1.6, 2.4)
	else:
		pod.count = randi_range(1, 2)
		pod.eject_speed = 540.0
		pod.warn_time = 0.55
		pod.tint = Color(1.4, 1.9, 1.2)
	get_parent().add_child(pod)

func _pick_scene() -> PackedScene:
	var basic_ok := Settings.enemy_basic_enabled and enemy_scene != null
	var wave_ok := Settings.enemy_wave_enabled and enemy_wave_scene != null
	var smart_ok := Settings.enemy_smart_enabled and enemy_smart_scene != null
	if not basic_ok and not wave_ok and not smart_ok:
		return null
	var wave_w := clampf((_elapsed - 20.0) / 40.0, 0.0, 0.35)
	var smart_w := clampf((_elapsed - 60.0) / 60.0, 0.0, 0.25)
	var r := randf()
	if r < smart_w and smart_ok:
		return enemy_smart_scene
	if r < smart_w + wave_w and wave_ok:
		return enemy_wave_scene
	if basic_ok:
		return enemy_scene
	if wave_ok:
		return enemy_wave_scene
	return enemy_smart_scene
