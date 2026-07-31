extends CanvasLayer
## In-game pause surface. This layer keeps processing while SceneTree is paused.

var _panel: Control

func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	layer = 6
	_build_ui()
	_apply_safe_area.call_deferred()

func _build_ui() -> void:
	var pause_button := Button.new()
	pause_button.text = "Ⅱ"
	pause_button.tooltip_text = "Pause"
	pause_button.add_theme_font_size_override("font_size", 52)
	pause_button.anchor_left = 1.0
	pause_button.anchor_right = 1.0
	pause_button.offset_left = -150.0
	pause_button.offset_right = -38.0
	pause_button.offset_top = 34.0
	pause_button.offset_bottom = 142.0
	pause_button.pressed.connect(open_menu)
	add_child(pause_button)

	_panel = Control.new()
	_panel.visible = false
	_panel.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT, Control.PRESET_MODE_MINSIZE, 0)
	_panel.mouse_filter = Control.MOUSE_FILTER_STOP
	add_child(_panel)

	var shade := ColorRect.new()
	shade.color = Color(0.01, 0.01, 0.04, 0.82)
	shade.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	shade.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_panel.add_child(shade)

	var box := VBoxContainer.new()
	box.set_anchors_preset(Control.PRESET_CENTER)
	box.offset_left = -260.0
	box.offset_top = -360.0
	box.offset_right = 260.0
	box.offset_bottom = 360.0
	box.add_theme_constant_override("separation", 28)
	_panel.add_child(box)

	var title := Label.new()
	title.text = "PAUSED"
	title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	title.add_theme_font_size_override("font_size", 76)
	box.add_child(title)

	_button(box, "RESUME", close_menu)
	_button(box, "RESTART", _restart)
	_button(box, "SETTINGS", _settings)
	_button(box, "EXIT TO TITLE", _exit_to_title)

func _apply_safe_area() -> void:
	if OS.get_name() != "iOS" and OS.get_name() != "Android":
		return
	var safe := DisplayServer.get_display_safe_area()
	var win := DisplayServer.window_get_size()
	if safe.size.x <= 0.0 or safe.size.y <= 0.0 or win.y <= 0.0:
		return
	var visible := get_viewport().get_visible_rect().size
	var sx := visible.x / float(win.x)
	var sy := visible.y / float(win.y)
	# Keep the pause affordance outside the Dynamic Island/notch and right edge.
	var button := get_child(0) as Control
	button.offset_top = maxf(34.0, safe.position.y * sy + 18.0)
	button.offset_right = -maxf(38.0, (win.x - (safe.position.x + safe.size.x)) * sx + 18.0)

func _button(parent: VBoxContainer, caption: String, action: Callable) -> void:
	var b := Button.new()
	b.text = caption
	b.custom_minimum_size = Vector2(0, 112)
	b.add_theme_font_size_override("font_size", 42)
	b.pressed.connect(action)
	parent.add_child(b)

func open_menu() -> void:
	_panel.visible = true
	get_tree().paused = true

func close_menu() -> void:
	get_tree().paused = false
	_panel.visible = false

func _restart() -> void:
	get_tree().paused = false
	get_tree().reload_current_scene()

func _settings() -> void:
	get_tree().paused = false
	Main.demo_mode = false
	get_tree().change_scene_to_file("res://scenes/Settings.tscn")

func _exit_to_title() -> void:
	get_tree().paused = false
	Main.demo_mode = false
	get_tree().change_scene_to_file("res://scenes/TitleScreen.tscn")
