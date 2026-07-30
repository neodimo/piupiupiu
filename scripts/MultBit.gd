extends Node2D
class_name MultBit
## Multiplier bit — mirrors Unity's Multiplier.cs. Emitted from dead enemies
## with a random outward velocity; drifts and slows; once the player comes
## within pickup range it homes toward them and accelerates. Collecting one
## bumps the run multiplier. Uncollected bits fade out after a few seconds.
## Drawn as a small glowing diamond (HDR-boosted so it blooms).

@export var homing_speed_min: float = 620.0
@export var homing_speed_max: float = 820.0
@export var accel: float = 90.0
@export var pickup_range: float = 420.0   # start homing when player is this close
@export var collect_range: float = 46.0
@export var lifetime: float = 6.0
@export var arm_delay: float = 0.35       # brief delay before it can be sucked in

var _vel: Vector2
var _actual_speed: float = 0.0
var _rand_speed: float
var _age: float = 0.0
var _color := Color(1.8, 1.4, 3.2)        # neon violet, HDR

func launch(initial_vel: Vector2) -> void:
	_vel = initial_vel

func _ready() -> void:
	_rand_speed = randf_range(homing_speed_min, homing_speed_max)
	if _vel == Vector2.ZERO:
		_vel = Vector2(randf_range(-1, 1), randf_range(-1, 1)).normalized() * randf_range(120, 260)

func _physics_process(delta: float) -> void:
	_age += delta
	if _age >= lifetime:
		_fade_and_free()
		set_physics_process(false)
		return

	var player := Player.instance
	if player != null and _age > arm_delay:
		var to_player := player.global_position - global_position
		var dist := to_player.length()
		if dist <= collect_range:
			_collect()
			return
		if dist <= pickup_range:
			# accelerate homing pull, like Multiplier.cs GoToPlayer()
			if _actual_speed < _rand_speed:
				_actual_speed += accel
			global_position = global_position.move_toward(player.global_position, _actual_speed * delta)
			queue_redraw()
			return

	# free drift with drag
	_vel = _vel.move_toward(Vector2.ZERO, 220.0 * delta)
	global_position += _vel * delta
	rotation += delta * 3.0
	queue_redraw()

func _collect() -> void:
	GameSession.bump_multiplier()
	Settings.buzz(8)
	var grid := get_tree().get_first_node_in_group("spring_grid")
	if grid != null and grid.has_method("disturb"):
		grid.disturb(global_position, 260.0, 120.0)
	# quick pop
	var p := CPUParticles2D.new()
	get_parent().add_child(p)
	p.global_position = global_position
	p.emitting = true
	p.one_shot = true
	p.explosiveness = 1.0
	p.amount = 6
	p.lifetime = 0.3
	p.initial_velocity_min = 80.0
	p.initial_velocity_max = 200.0
	p.spread = 180.0
	p.scale_amount_min = 2.0
	p.scale_amount_max = 3.0
	p.color = Color(1.6, 1.3, 3.0)
	p.get_tree().create_timer(0.4).timeout.connect(p.queue_free)
	queue_free()

func _fade_and_free() -> void:
	var t := create_tween()
	t.tween_property(self, "modulate:a", 0.0, 0.4)
	t.tween_callback(queue_free)

func _draw() -> void:
	# glowing diamond with a brighter core
	var s := 11.0
	var pts := PackedVector2Array([
		Vector2(0, -s), Vector2(s, 0), Vector2(0, s), Vector2(-s, 0)
	])
	draw_colored_polygon(pts, _color)
	var core := PackedVector2Array([
		Vector2(0, -s * 0.5), Vector2(s * 0.5, 0), Vector2(0, s * 0.5), Vector2(-s * 0.5, 0)
	])
	draw_colored_polygon(core, Color(2.6, 2.4, 3.6))
