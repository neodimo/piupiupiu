extends Area2D
## Player laser. Pooled-in-spirit (freed on hit/exit); mirrors laserPrefab.

@export var speed: float = 1600.0
var _dir: Vector2 = Vector2.UP
var _damage: float = 50.0
var _pierces_left: int = 0
var _ripple_time: float = 0.0

func setup(dir: Vector2, damage: float = 50.0, pierces: int = 0) -> void:
	_dir = dir
	_damage = damage
	_pierces_left = pierces
	rotation = dir.angle() + PI * 0.5

func _physics_process(delta: float) -> void:
	global_position += _dir * speed * delta
	# Lasers comb the surface as they travel. Sampling instead of firing every
	# physics tick preserves the heavy feeling without flooding the grid solver.
	_ripple_time -= delta
	if _ripple_time <= 0.0:
		_ripple_time = 0.075
		var grid := get_tree().get_first_node_in_group("spring_grid")
		if grid != null and grid.has_method("disturb"):
			grid.disturb(global_position, 92.0, 115.0)

func _on_area_entered(a: Area2D) -> void:
	if a.is_in_group("enemies"):
		var grid := get_tree().get_first_node_in_group("spring_grid")
		if grid != null and grid.has_method("disturb"):
			grid.disturb(global_position, 520.0, 185.0)
		if a.has_method("take_damage"):
			a.take_damage(_damage)
		if _pierces_left <= 0:
			queue_free()
		else:
			_pierces_left -= 1

func _on_screen_exited() -> void:
	queue_free()
