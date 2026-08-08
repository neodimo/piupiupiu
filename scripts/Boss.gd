extends Area2D
## Prism Warden: a high-health arena boss inspired by the original's large
## emitter pressure. It advances in pulses and sends broad grid shockwaves.

@export var max_health: float = 2400.0
@export var points: int = 350
var _health: float
var _phase: float = 0.0
var _pulse: float = 0.0

func _ready() -> void:
	_health = max_health
	add_to_group("enemies")
	queue_redraw()

func _physics_process(delta: float) -> void:
	var player := Player.instance
	if player == null:
		return
	_phase += delta
	_pulse -= delta
	var target := player.global_position + Vector2(cos(_phase * 0.55), sin(_phase * 0.7)) * 440.0
	global_position = global_position.move_toward(target, 125.0 * delta)
	if _pulse <= 0.0:
		_pulse = 2.4
		var grid := get_tree().get_first_node_in_group("spring_grid")
		if grid and grid.has_method("disturb"):
			grid.disturb(global_position, 2800.0, 220.0)
		Settings.buzz(40)
	queue_redraw()

func take_damage(amount: float) -> void:
	_health -= amount
	if _health <= 0.0:
		GameSession.add_points(points)
		var grid := get_tree().get_first_node_in_group("spring_grid")
		if grid and grid.has_method("disturb"):
			grid.disturb(global_position, 6200.0, 700.0)
		Main.spawn_death_vfx(global_position, Color(2.4, 0.45, 1.9), true)
		Main.spawn_score_popup(global_position, points * GameSession.multiplier, Color(2.5, 0.8, 2.0))
		Main.spawn_mult_bits(global_position, 12, 18)
		queue_free()

func _draw() -> void:
	var beat := 1.0 + sin(_phase * 4.0) * 0.10
	draw_circle(Vector2.ZERO, 115.0 * beat, Color(1.5, 0.12, 1.8, 0.16))
	draw_circle(Vector2.ZERO, 78.0, Color(0.25, 0.04, 0.42, 1.0))
	draw_arc(Vector2.ZERO, 100.0, _phase, _phase + PI * 1.55, 32, Color(2.4, 0.25, 2.1, 1.0), 9.0)
	draw_arc(Vector2.ZERO, 72.0, -_phase * 1.4, -_phase * 1.4 + PI * 1.45, 32, Color(0.3, 1.6, 2.5, 1.0), 8.0)
	draw_circle(Vector2.ZERO, 29.0 + sin(_phase * 6.0) * 5.0, Color(2.6, 0.8, 2.4, 1.0))
	# compact health gauge directly above the boss
	draw_rect(Rect2(-100, -148, 200, 12), Color(0.05, 0.02, 0.10, 0.9))
	draw_rect(Rect2(-98, -146, 196 * maxf(_health, 0.0) / max_health, 8), Color(2.2, 0.25, 1.9, 1.0))
