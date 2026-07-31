extends Area2D
## Wave enemy — ejected straight out of the elongated emitter, then travels in a
## strict straight line across the field. When it reaches the far side it turns
## 180° and retraces its path, back and forth. Its narrow pointy front always
## faces the direction it's moving. A row of them = a wall you weave through.
## Own art: 2-frame neon triangle.

@export var patrol_speed: float = 260.0
@export var max_health: float = 60.0
@export var points: int = 10
@export var bounds: Vector2 = Vector2(900, 1650)

var _health: float
var _dir: Vector2 = Vector2.DOWN            # strict straight travel direction

func spawn_launch(_vel: Vector2, _index: int, _count: int, eject_dir: Vector2) -> void:
	# travel straight inward, exactly along the emitter's outward normal
	_dir = eject_dir.normalized()
	_orient()

func _ready() -> void:
	_health = max_health
	add_to_group("enemies")
	var sprite := $Sprite as AnimatedSprite2D
	sprite.sprite_frames = SheetAnim.build_from_textures([
		load("res://art/enemy_wave_1.png"), load("res://art/enemy_wave_2.png")
	], 5.0)
	sprite.frame = randi() % 2
	sprite.play("default")
	_orient()

## Point the narrow (pointy) side of the triangle in the travel direction.
## The art's point faces up (-Y) at zero rotation, so add a quarter turn.
func _orient() -> void:
	($Sprite as Node2D).rotation = _dir.angle() + PI * 0.5

func _physics_process(delta: float) -> void:
	global_position += _dir * patrol_speed * delta
	# turn 180° at the far side and keep going
	if _dir.x > 0.0 and global_position.x > bounds.x:
		global_position.x = bounds.x; _dir = -_dir; _orient()
	elif _dir.x < 0.0 and global_position.x < -bounds.x:
		global_position.x = -bounds.x; _dir = -_dir; _orient()
	elif _dir.y > 0.0 and global_position.y > bounds.y:
		global_position.y = bounds.y; _dir = -_dir; _orient()
	elif _dir.y < 0.0 and global_position.y < -bounds.y:
		global_position.y = -bounds.y; _dir = -_dir; _orient()

func take_damage(amount: float) -> void:
	_health -= amount
	modulate = Color(2.5, 2.5, 2.5)
	if _health <= 0.0:
		_die()
	else:
		var t := create_tween()
		t.tween_property(self, "modulate", Color.WHITE, 0.12)

func _die() -> void:
	GameSession.add_points(points)
	var grid := get_tree().get_first_node_in_group("spring_grid")
	if grid != null and grid.has_method("disturb"):
		grid.disturb(global_position, 700.0)
	Main.spawn_death_vfx(global_position, Color(1.0, 0.4, 1.4), true)
	Main.spawn_score_popup(global_position, points * GameSession.multiplier, Color(1.0, 0.5, 1.6))
	Main.spawn_mult_bits(global_position, 2, 5)
	Settings.buzz(18)
	queue_free()
