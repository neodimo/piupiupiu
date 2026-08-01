extends CanvasLayer
class_name UpgradeMenu
## Level-up pause and upgrade selection. Built in code to remain independent of
## the game HUD scene; it processes while the gameplay tree is paused.

signal chosen(kind: String)

var _panel: ColorRect
var _title: Label
var _cards: Array[Button] = []
const UPGRADES := [
	{"kind":"drone", "name":"ORBIT DRONE", "detail":"A neon satellite joins your firing pattern", "art":"res://art/upgrade_drone.png"},
	{"kind":"drone_swarm", "name":"DRONE SWARM", "detail":"Deploy two more orbiting attack drones", "art":"res://art/upgrade_drone.png"},
	{"kind":"spread", "name":"TWIN LASER", "detail":"Add another forward laser to every volley", "art":"res://art/upgrade_laser.png"},
	{"kind":"pierce", "name":"PHASE PIERCE", "detail":"Lasers punch through one additional enemy", "art":"res://art/upgrade_laser.png"},
	{"kind":"overcharge", "name":"OVERCHARGE", "detail":"15% faster auto-fire", "art":"res://art/upgrade_overcharge.png"},
	{"kind":"amplify", "name":"AMPLIFIER CORE", "detail":"Increase every laser's damage", "art":"res://art/upgrade_overcharge.png"},
]

func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	_panel = ColorRect.new()
	_panel.color = Color(0.025, 0.015, 0.08, 0.92)
	_panel.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	_panel.mouse_filter = Control.MOUSE_FILTER_STOP
	add_child(_panel)
	var box := VBoxContainer.new()
	box.set_anchors_preset(Control.PRESET_CENTER)
	box.position = Vector2(-470, -510)
	box.size = Vector2(940, 1020)
	box.add_theme_constant_override("separation", 24)
	_panel.add_child(box)
	_title = Label.new()
	_title.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	_title.add_theme_font_size_override("font_size", 72)
	box.add_child(_title)
	var hint := Label.new()
	hint.text = "CHOOSE ONE MUTATION"
	hint.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	hint.add_theme_font_size_override("font_size", 28)
	box.add_child(hint)
	for upgrade in _draw_choices():
		_add_card(box, upgrade)
	visible = false

func _draw_choices() -> Array:
	var pool := UPGRADES.duplicate()
	pool.shuffle()
	return [pool[0], pool[1], pool[2]]

func _add_card(box: VBoxContainer, upgrade: Dictionary) -> void:
	var button := Button.new()
	button.custom_minimum_size = Vector2(940, 245)
	button.tooltip_text = upgrade["detail"]
	var row := HBoxContainer.new()
	row.mouse_filter = Control.MOUSE_FILTER_IGNORE
	row.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT, Control.PRESET_MODE_MINSIZE, 18)
	var art := TextureRect.new()
	art.texture = load(upgrade["art"])
	art.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	art.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
	art.custom_minimum_size = Vector2(210, 210)
	art.mouse_filter = Control.MOUSE_FILTER_IGNORE
	row.add_child(art)
	var copy := VBoxContainer.new()
	copy.mouse_filter = Control.MOUSE_FILTER_IGNORE
	copy.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	var name := Label.new()
	name.text = upgrade["name"]
	name.add_theme_font_size_override("font_size", 42)
	name.mouse_filter = Control.MOUSE_FILTER_IGNORE
	copy.add_child(name)
	var detail := Label.new()
	detail.text = upgrade["detail"]
	detail.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	detail.add_theme_font_size_override("font_size", 26)
	detail.mouse_filter = Control.MOUSE_FILTER_IGNORE
	copy.add_child(detail)
	row.add_child(copy)
	button.add_child(row)
	button.pressed.connect(func(): _choose(upgrade["kind"]))
	box.add_child(button)
	_cards.append(button)

func present(level: int) -> void:
	_title.text = "LEVEL %d" % level
	visible = true
	get_tree().paused = true

func _choose(kind: String) -> void:
	visible = false
	get_tree().paused = false
	chosen.emit(kind)
