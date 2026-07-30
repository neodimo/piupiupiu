extends Node2D
class_name Emitter
## Spawn pod — mirrors the Unity EnemySpawner "tile" that enemies are ejected
## from. Appears at a play-field edge, telegraphs with a pulsing glow (so the
## player sees where the wave is coming from), then ejects its enemies outward
## with a launch velocity and fades away. Uses the original waveSpawner pill art.

@export var enemy_scene: PackedScene
@export var count: int = 1
@export var warn_time: float = 0.7
@export var eject_dir: Vector2 = Vector2.DOWN
@export var eject_speed: float = 520.0
@export var spread: float = 120.0          # perpendicular spacing between ejected enemies
@export var tint: Color = Color(1.6, 1.2, 2.4)

var _sprite: Sprite2D

func _ready() -> void:
	_sprite = Sprite2D.new()
	_sprite.texture = load("res://art/emitter.png")
	_sprite.modulate = tint
	# orient the pill so its long axis lies along the edge (perpendicular to eject)
	_sprite.rotation = eject_dir.angle() + PI * 0.5
	_sprite.scale = Vector2(0.1, 0.1)
	add_child(_sprite)

	var grid := get_tree().get_first_node_in_group("spring_grid")
	if grid != null and grid.has_method("disturb"):
		grid.disturb(global_position, 340.0, 220.0)

	# telegraph: pop in, pulse, then eject
	var t := create_tween()
	t.tween_property(_sprite, "scale", Vector2(0.42, 0.42), 0.22).set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT)
	t.tween_property(_sprite, "modulate:a", 0.55, 0.18)
	t.tween_property(_sprite, "modulate:a", 1.0, 0.18)
	t.tween_interval(maxf(0.0, warn_time - 0.58))
	t.tween_callback(_eject)
	t.tween_property(_sprite, "scale", Vector2(0.52, 0.52), 0.12)
	t.parallel().tween_property(_sprite, "modulate:a", 0.0, 0.35)
	t.tween_callback(queue_free)

func _eject() -> void:
	if enemy_scene == null:
		return
	var perp := eject_dir.orthogonal()
	var half := (count - 1) * 0.5
	for i in count:
		var e := enemy_scene.instantiate()
		get_parent().add_child(e)
		e.global_position = global_position + perp * (i - half) * spread
		var vel := eject_dir * eject_speed
		if e.has_method("spawn_launch"):
			e.spawn_launch(vel, i, count, eject_dir)
		elif "_spawn_vel" in e:
			e.set("_spawn_vel", vel)
	# kick the grid on ejection
	var grid := get_tree().get_first_node_in_group("spring_grid")
	if grid != null and grid.has_method("disturb"):
		grid.disturb(global_position, 620.0, 260.0)
	Settings.buzz(10)
