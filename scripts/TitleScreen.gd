extends Node2D
## Title screen — tap anywhere or press any key to start.

func _ready() -> void:
	var grid := $SpringGrid
	if grid and grid.has_method("disturb"):
		get_tree().create_timer(0.4).timeout.connect(func():
			grid.disturb(Vector2.ZERO, 600.0, 400.0))
	var tap_label: Label = $HUD/Tap
	var t := create_tween().set_loops()
	t.tween_property(tap_label, "modulate:a", 0.2, 0.9)
	t.tween_property(tap_label, "modulate:a", 1.0, 0.9)

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenTouch and event.pressed:
		_start()
	elif event is InputEventMouseButton and event.pressed:
		_start()
	elif event is InputEventKey and event.pressed:
		_start()

func _start() -> void:
	get_tree().change_scene_to_file("res://scenes/Main.tscn")
