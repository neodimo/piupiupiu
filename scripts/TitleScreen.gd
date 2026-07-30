extends Node2D
## Title screen — the game runs as an unfocused blurred demo behind an animated
## "piu piu piu" neon logo. Tap anywhere (except the settings button) to play.

func _ready() -> void:
	# spin up the background demo BEFORE it enters the tree so its _ready sees demo mode
	Main.demo_mode = true
	var demo: Node = load("res://scenes/Main.tscn").instantiate()
	add_child(demo)
	move_child(demo, 0)   # keep it underneath the blur + UI

	# animated logo: gentle bob + breathing pulse
	var logo: TextureRect = $UI/Logo
	logo.pivot_offset = logo.size * 0.5
	var bob := create_tween().set_loops()
	bob.tween_property(logo, "position:y", logo.position.y + 26.0, 1.6).set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_IN_OUT)
	bob.tween_property(logo, "position:y", logo.position.y, 1.6).set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_IN_OUT)
	var pulse := create_tween().set_loops()
	pulse.tween_property(logo, "scale", Vector2(1.05, 1.05), 1.1).set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_IN_OUT)
	pulse.tween_property(logo, "scale", Vector2(1.0, 1.0), 1.1).set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_IN_OUT)

	var tap_label: Label = $UI/Tap
	var t := create_tween().set_loops()
	t.tween_property(tap_label, "modulate:a", 0.25, 0.9)
	t.tween_property(tap_label, "modulate:a", 1.0, 0.9)

	$UI/SettingsBtn.pressed.connect(_open_settings)

func _open_settings() -> void:
	Main.demo_mode = false
	get_tree().change_scene_to_file("res://scenes/Settings.tscn")

func _unhandled_input(event: InputEvent) -> void:
	var go := false
	if event is InputEventScreenTouch and event.pressed:
		go = true
	elif event is InputEventMouseButton and event.pressed:
		go = true
	elif event is InputEventKey and event.pressed:
		go = true
	if go:
		_start()

func _start() -> void:
	Main.demo_mode = false
	get_tree().change_scene_to_file("res://scenes/Main.tscn")
