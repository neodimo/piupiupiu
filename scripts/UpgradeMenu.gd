extends CanvasLayer
class_name UpgradeMenu
## Level-up pause and upgrade selection. Built in code to remain independent of
## the game HUD scene; it processes while the gameplay tree is paused.

signal chosen(kind: String)

var _panel: ColorRect
var _title: Label
var _buttons: Array[Button] = []

func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	_panel = ColorRect.new()
	_panel.color = Color(0.025, 0.015, 0.08, 0.92)
	_panel.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	_panel.mouse_filter = Control.MOUSE_FILTER_STOP
	add_child(_panel)
	var box := VBoxContainer.new()
	box.set_anchors_preset(Control.PRESET_CENTER)
	box.position = Vector2(-360, -350)
	box.size = Vector2(720, 700)
	box.add_theme_constant_override("separation", 24)
	_panel.add_child(box)
	_title = Label.new()
	_title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	_title.add_theme_font_size_override("font_size", 64)
	box.add_child(_title)
	var hint := Label.new()
	hint.text = "CHOOSE ONE MUTATION"
	hint.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	hint.add_theme_font_size_override("font_size", 28)
	box.add_child(hint)
	_add_button(box, "drone", "ORBIT DRONE", "A neon satellite joins your firing pattern")
	_add_button(box, "spread", "TWIN LASER", "Add one more forward laser")
	_add_button(box, "overcharge", "OVERCHARGE", "15% faster auto-fire")
	visible = false

func _add_button(box: VBoxContainer, kind: String, name: String, detail: String) -> void:
	var button := Button.new()
	button.custom_minimum_size = Vector2(720, 118)
	button.text = name + "\n" + detail
	button.add_theme_font_size_override("font_size", 30)
	button.pressed.connect(func(): _choose(kind))
	box.add_child(button)
	_buttons.append(button)

func present(level: int) -> void:
	_title.text = "LEVEL %d" % level
	visible = true
	get_tree().paused = true

func _choose(kind: String) -> void:
	visible = false
	get_tree().paused = false
	chosen.emit(kind)
