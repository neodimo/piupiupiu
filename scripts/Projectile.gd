extends Area2D
## Player laser. Pooled-in-spirit (freed on hit/exit); mirrors laserPrefab.

@export var speed: float = 1600.0
var _dir: Vector2 = Vector2.UP

func setup(dir: Vector2) -> void:
	_dir = dir
	rotation = dir.angle() + PI * 0.5

func _physics_process(delta: float) -> void:
	global_position += _dir * speed * delta

func _on_area_entered(a: Area2D) -> void:
	if a.is_in_group("enemies"):
		if a.has_method("take_damage"):
			a.take_damage(50)
		queue_free()

func _on_screen_exited() -> void:
	queue_free()
