extends Node2D
## Settings page. Runs the game as a live (dimmed) background so post-processing
## changes are visible in real time while you dial in a sweet spot. Audio,
## haptics, the full post-FX stack, and a god-mode toggle — all persisted.

var _demo: Node
var _preview: AudioStreamPlayer

func _ready() -> void:
	# live background demo so FX sliders show their effect immediately
	Main.demo_mode = true
	_demo = load("res://scenes/Main.tscn").instantiate()
	add_child(_demo)
	move_child(_demo, 0)

	var layer := CanvasLayer.new()
	layer.layer = 5
	add_child(layer)

	# dim scrim so the panel is readable over the game
	var scrim := ColorRect.new()
	scrim.color = Color(0.02, 0.02, 0.05, 0.55)
	scrim.anchor_right = 1.0
	scrim.anchor_bottom = 1.0
	scrim.mouse_filter = Control.MOUSE_FILTER_IGNORE
	layer.add_child(scrim)

	var scroll := ScrollContainer.new()
	scroll.anchor_right = 1.0
	scroll.anchor_bottom = 1.0
	scroll.offset_left = 50
	scroll.offset_top = 60
	scroll.offset_right = -50
	scroll.offset_bottom = -50
	scroll.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED
	layer.add_child(scroll)

	var vbox := VBoxContainer.new()
	vbox.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	vbox.add_theme_constant_override("separation", 34)
	scroll.add_child(vbox)

	_title(vbox, "SETTINGS", 84)

	_title(vbox, "AUDIO", 40)
	_slider(vbox, "Music", Settings.music_volume, 0.0, 1.0, 0.01, func(v):
		Settings.music_volume = v
		if _preview: _preview.volume_db = Settings.music_db())
	_slider(vbox, "SFX", Settings.sfx_volume, 0.0, 1.0, 0.01, func(v): Settings.sfx_volume = v)
	_toggle(vbox, "Haptics", Settings.haptics, func(on):
		Settings.haptics = on
		Settings.buzz(20))

	_title(vbox, "POST-PROCESSING", 40)
	_slider(vbox, "Glow", Settings.fx_glow, 0.0, 3.0, 0.05, func(v):
		Settings.fx_glow = v
		_reapply())
	_slider(vbox, "Bloom", Settings.fx_bloom, 0.0, 0.6, 0.01, func(v):
		Settings.fx_bloom = v
		_reapply())
	_slider(vbox, "Aberration", Settings.fx_aberration, 0.0, 5.0, 0.1, func(v):
		Settings.fx_aberration = v
		_reapply())
	_slider(vbox, "Vignette", Settings.fx_vignette, 0.0, 1.0, 0.01, func(v):
		Settings.fx_vignette = v
		_reapply())
	_slider(vbox, "Grain", Settings.fx_grain, 0.0, 0.08, 0.002, func(v):
		Settings.fx_grain = v
		_reapply())
	_slider(vbox, "Saturation", Settings.fx_saturation, 0.0, 2.0, 0.01, func(v):
		Settings.fx_saturation = v
		_reapply())
	_slider(vbox, "Contrast", Settings.fx_contrast, 0.5, 1.8, 0.01, func(v):
		Settings.fx_contrast = v
		_reapply())
	_slider(vbox, "Brightness", Settings.fx_brightness, -0.25, 0.25, 0.01, func(v):
		Settings.fx_brightness = v
		_reapply())
	_slider(vbox, "Lens / Barrel", Settings.fx_lens_distortion, -0.35, 0.35, 0.01, func(v):
		Settings.fx_lens_distortion = v
		_reapply())

	_title(vbox, "PLAYER FEEL", 40)
	_slider(vbox, "Grid Distortion", Settings.player_distortion, 4.0, 35.0, 0.5, func(v):
		Settings.player_distortion = v)

	_title(vbox, "ENEMY TESTING", 40)
	_toggle(vbox, "Basic / Green", Settings.enemy_basic_enabled, func(on):
		Settings.enemy_basic_enabled = on)
	_toggle(vbox, "Wave", Settings.enemy_wave_enabled, func(on):
		Settings.enemy_wave_enabled = on)
	_toggle(vbox, "Smart", Settings.enemy_smart_enabled, func(on):
		Settings.enemy_smart_enabled = on)
	_toggle(vbox, "Boss: Prism Warden", Settings.enemy_boss_enabled, func(on):
		Settings.enemy_boss_enabled = on)

	_title(vbox, "DEBUG", 40)
	_toggle(vbox, "God Mode", Settings.god_mode, func(on): Settings.god_mode = on)

	var back := Button.new()
	back.text = "BACK"
	back.add_theme_font_size_override("font_size", 52)
	back.custom_minimum_size = Vector2(0, 110)
	back.pressed.connect(_on_back)
	vbox.add_child(back)

	_preview = AudioStreamPlayer.new()
	_preview.stream = load("res://art/venus.wav")
	_preview.volume_db = Settings.music_db()
	add_child(_preview)
	_preview.play()

## Re-push post-FX into the live demo so slider changes show immediately.
func _reapply() -> void:
	if _demo and _demo.has_method("_apply_post_fx"):
		_demo._apply_post_fx()

func _title(parent: Node, text: String, size: int) -> void:
	var l := Label.new()
	l.text = text
	l.add_theme_font_size_override("font_size", size)
	parent.add_child(l)

func _slider(parent: Node, name: String, value: float, lo: float, hi: float, step: float, on_change: Callable) -> void:
	var row := HBoxContainer.new()
	row.add_theme_constant_override("separation", 24)
	var lbl := Label.new()
	lbl.text = name
	lbl.custom_minimum_size = Vector2(390, 0)
	lbl.add_theme_font_size_override("font_size", 40)
	row.add_child(lbl)
	var s := HSlider.new()
	s.min_value = lo
	s.max_value = hi
	s.step = step
	s.value = value
	s.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	s.size_flags_vertical = Control.SIZE_SHRINK_CENTER
	s.custom_minimum_size = Vector2(0, 60)
	var val := Label.new()
	val.custom_minimum_size = Vector2(130, 0)
	val.horizontal_alignment = HORIZONTAL_ALIGNMENT_RIGHT
	val.add_theme_font_size_override("font_size", 34)
	val.text = "%.2f" % value
	s.value_changed.connect(func(v):
		val.text = "%.2f" % v
		on_change.call(v))
	row.add_child(s)
	row.add_child(val)
	parent.add_child(row)

func _toggle(parent: Node, name: String, value: bool, on_change: Callable) -> void:
	var row := HBoxContainer.new()
	row.add_theme_constant_override("separation", 24)
	var lbl := Label.new()
	lbl.text = name
	lbl.custom_minimum_size = Vector2(580, 0)
	lbl.add_theme_font_size_override("font_size", 40)
	row.add_child(lbl)
	var chk := CheckButton.new()
	chk.custom_minimum_size = Vector2(160, 88)
	chk.add_theme_icon_override("checked", _toggle_icon(true))
	chk.add_theme_icon_override("unchecked", _toggle_icon(false))
	chk.button_pressed = value
	chk.toggled.connect(func(on): on_change.call(on))
	row.add_child(chk)
	parent.add_child(row)

func _toggle_icon(on: bool) -> ImageTexture:
	var img := Image.create(130, 72, false, Image.FORMAT_RGBA8)
	img.fill(Color(0.10, 0.14, 0.24, 1.0) if not on else Color(0.18, 0.75, 0.55, 1.0))
	var knob_x := 95 if on else 35
	for y in 72:
		for x in 130:
			var d := Vector2(x, y).distance_to(Vector2(knob_x, 36))
			if d < 27:
				img.set_pixel(x, y, Color(0.92, 0.98, 1.0, 1.0))
	return ImageTexture.create_from_image(img)

func _on_back() -> void:
	Settings.save_settings()
	Main.demo_mode = false
	get_tree().change_scene_to_file("res://scenes/TitleScreen.tscn")
