extends Node2D
class_name Main
## Root bootstrap: camera, grid, player, spawner, HUD wiring.

static var instance: Main
## When true this Main is an unfocused background demo (title screen): the
## player auto-plays and never dies, and the HUD/music stay quiet.
static var demo_mode: bool = false

var _score_label: Label
var _gameover_label: Label
var _exp_bar: ProgressBar
var _levelup_label: Label
var _bgm: AudioStreamPlayer

func _ready() -> void:
	instance = self
	_apply_post_fx()
	GameSession.reset()
	GameSession.score_changed.connect(_on_score_changed)
	GameSession.game_over.connect(_on_game_over)
	GameSession.exp_changed.connect(_on_exp_changed)
	GameSession.leveled_up.connect(_on_leveled_up)
	_score_label = $HUD/Score
	_gameover_label = $HUD/GameOver
	_exp_bar = $HUD/ExpBar
	_levelup_label = $HUD/LevelUp
	_gameover_label.visible = false
	_on_score_changed(0, 1)
	if demo_mode:
		$HUD.visible = false
	else:
		_start_music()
		_apply_safe_area.call_deferred()
	if OS.has_environment("PIU_CAPTURE"):
		_capture_after(float(OS.get_environment("PIU_CAPTURE")))

## Inset the HUD out of the iPhone Dynamic Island / notch (top) and the home
## indicator (bottom), using the OS-reported safe area mapped into viewport space.
## On devices with no cutout the insets are zero and nothing moves.
func _apply_safe_area() -> void:
	# Safe-area cutouts (Dynamic Island / notch / home indicator) only exist on
	# mobile; on desktop/headless the reported area is unrelated to the window,
	# so skip and let the HUD sit at the true screen edges.
	if OS.get_name() != "iOS" and OS.get_name() != "Android":
		return
	var win := DisplayServer.window_get_size()
	var safe := DisplayServer.get_display_safe_area()
	if win.x <= 0 or win.y <= 0 or safe.size.x <= 0 or safe.size.y <= 0:
		return
	var vis := get_viewport().get_visible_rect().size
	var sy := vis.y / float(win.y)
	var sx := vis.x / float(win.x)
	var cap_y := win.y * 0.2
	var cap_x := win.x * 0.2
	var top := clampf(safe.position.y, 0.0, cap_y) * sy
	var bottom := clampf(win.y - (safe.position.y + safe.size.y), 0.0, cap_y) * sy
	var left := clampf(safe.position.x, 0.0, cap_x) * sx
	if _score_label:
		_score_label.offset_left = 40.0 + left
		_score_label.offset_top = 30.0 + top
	if _exp_bar:
		_exp_bar.offset_top = -24.0 - bottom
		_exp_bar.offset_bottom = -bottom

## Push the dialable post-processing settings into the live environment + shader.
func _apply_post_fx() -> void:
	var we := get_node_or_null("WorldEnvironment") as WorldEnvironment
	if we and we.environment:
		we.environment.glow_intensity = Settings.fx_glow
		we.environment.glow_bloom = Settings.fx_bloom
	var screen := get_node_or_null("PostFX/Screen") as ColorRect
	if screen and screen.material:
		var m := screen.material as ShaderMaterial
		m.set_shader_parameter("aberration", Settings.fx_aberration)
		m.set_shader_parameter("vignette_strength", Settings.fx_vignette)
		m.set_shader_parameter("grain", Settings.fx_grain)

func _start_music() -> void:
	_bgm = AudioStreamPlayer.new()
	_bgm.stream = load("res://art/venus.wav")
	_bgm.volume_db = Settings.music_db()
	add_child(_bgm)
	_bgm.finished.connect(_bgm.play)
	_bgm.play()

func _capture_after(secs: float) -> void:
	await get_tree().create_timer(secs).timeout
	await RenderingServer.frame_post_draw
	var img := get_viewport().get_texture().get_image()
	img.save_png("res://_capture.png")
	get_tree().quit()

func _on_score_changed(score: int, mult: int) -> void:
	if _score_label:
		_score_label.text = "%d   x%d" % [score, mult]

func _on_exp_changed(current_exp: int, required_exp: int, _lv: int) -> void:
	if _exp_bar:
		_exp_bar.max_value = required_exp
		_exp_bar.value = current_exp

func _on_leveled_up(_lv: int) -> void:
	if _levelup_label:
		_levelup_label.modulate.a = 1.0
		var t := create_tween()
		t.tween_property(_levelup_label, "modulate:a", 0.0, 1.4)

func _on_game_over() -> void:
	if _bgm:
		var t := create_tween()
		t.tween_property(_bgm, "volume_db", -40.0, 1.2)
		t.tween_callback(_bgm.stop)
	if _gameover_label:
		var hs := GameSession.high_score
		var sc := GameSession.current_score
		_gameover_label.text = "GAME OVER\n%d" % sc
		if hs > sc:
			_gameover_label.text += "\nBEST  %d" % hs
		else:
			_gameover_label.text += "\nNEW BEST!"
		_gameover_label.visible = true
	Settings.buzz(120)
	get_tree().create_timer(3.0).timeout.connect(func(): get_tree().reload_current_scene())

## Floating "+N" score popup that rushes toward the camera — mirrors
## PopupScoreMovement.cs (which moved the text toward Camera.main). It punches
## in small, scales up dramatically while rising, then recedes and fades.
static func spawn_score_popup(pos: Vector2, points: int, color: Color = Color(1.6, 1.8, 1.0)) -> void:
	if instance == null:
		return
	var lbl := Label.new()
	lbl.text = "+%d" % points
	lbl.add_theme_font_size_override("font_size", 60)
	lbl.add_theme_color_override("font_color", color)
	lbl.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	lbl.z_index = 60
	lbl.pivot_offset = Vector2(60, 40)
	instance.add_child(lbl)
	lbl.global_position = pos + Vector2(-60, -40)
	lbl.scale = Vector2(0.15, 0.15)
	var t := lbl.create_tween()
	t.set_parallel(true)
	# rush toward camera: overshoot big fast, then settle — reads as "coming at you"
	t.tween_property(lbl, "scale", Vector2(2.2, 2.2), 0.28).set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT)
	t.tween_property(lbl, "global_position:y", lbl.global_position.y - 90.0, 0.28).set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_OUT)
	t.chain().set_parallel(true)
	t.tween_property(lbl, "scale", Vector2(1.4, 1.4), 0.45)
	t.tween_property(lbl, "global_position:y", lbl.global_position.y - 180.0, 0.45)
	t.tween_property(lbl, "modulate:a", 0.0, 0.45).set_delay(0.12)
	t.chain().tween_callback(lbl.queue_free)

## Per-enemy death VFX — faithful to the Unity enemyDeath prefab: a burst of
## small plain glowing dots that fly out fast and fade (NOT the star sprite;
## that Explosion-stars art was a separate effect). Round dots read as soft
## glows through the bloom. Tinted to the enemy; `big` for the tougher types.
static var _dot_tex: Texture2D
static func _dot() -> Texture2D:
	if _dot_tex == null:
		var img := Image.create(24, 24, false, Image.FORMAT_RGBA8)
		var c := Vector2(11.5, 11.5)
		for y in 24:
			for x in 24:
				var d := Vector2(x, y).distance_to(c) / 12.0
				var a := clampf(1.0 - d, 0.0, 1.0)
				a = a * a   # soft falloff
				img.set_pixel(x, y, Color(1, 1, 1, a))
		_dot_tex = ImageTexture.create_from_image(img)
	return _dot_tex

static func spawn_death_vfx(pos: Vector2, color: Color = Color(0.5, 0.9, 1.0), big: bool = false) -> void:
	if instance == null:
		return
	var p := CPUParticles2D.new()
	instance.add_child(p)
	p.global_position = pos
	p.texture = _dot()
	p.emitting = true
	p.one_shot = true
	p.explosiveness = 1.0
	p.amount = 26 if big else 18
	p.lifetime = 0.6 if big else 0.45
	p.initial_velocity_min = 200.0
	p.initial_velocity_max = 640.0 if big else 480.0
	p.spread = 180.0
	p.damping_min = 120.0
	p.damping_max = 220.0
	p.scale_amount_min = 2.2 if big else 1.6
	p.scale_amount_max = 5.0 if big else 3.4
	var sc := Curve.new()
	sc.add_point(Vector2(0.0, 1.0))
	sc.add_point(Vector2(1.0, 0.0))
	p.scale_amount_curve = sc
	p.color = color
	var g := Gradient.new()
	g.set_color(0, Color(color.r, color.g, color.b, 1.0))
	g.set_color(1, Color(color.r, color.g, color.b, 0.0))
	p.color_ramp = g
	p.get_tree().create_timer(1.0).timeout.connect(p.queue_free)

## Emit N collectible multiplier bits from a dead enemy (Enemy.cs EmitMultipliers()).
static func spawn_mult_bits(pos: Vector2, count_min: int = 1, count_max: int = 5) -> void:
	if instance == null:
		return
	var count := randi_range(count_min, count_max)
	for i in count:
		var bit := MultBit.new()
		instance.add_child(bit)
		bit.global_position = pos + Vector2(randf_range(-24, 24), randf_range(-24, 24))
		bit.launch(Vector2(randf_range(-1, 1), randf_range(-1, 1)).normalized() * randf_range(160, 340))

static func spawn_explosion(pos: Vector2) -> void:
	if instance == null:
		return
	var p := CPUParticles2D.new()
	p.global_position = pos
	p.emitting = true
	p.one_shot = true
	p.explosiveness = 1.0
	p.amount = 24
	p.lifetime = 0.5
	p.initial_velocity_min = 220.0
	p.initial_velocity_max = 520.0
	p.spread = 180.0
	p.direction = Vector2.UP
	p.scale_amount_min = 3.0
	p.scale_amount_max = 6.0
	p.color = Color(0.5, 0.9, 1.0)
	instance.add_child(p)
	p.get_tree().create_timer(0.8).timeout.connect(p.queue_free)


## Expanding, fading neon ring drawn on death — the classic Geometry Wars pop.
class _Shockwave extends Node2D:
	var color: Color = Color(0.5, 0.9, 1.0)
	var _r: float = 8.0
	var _life: float = 0.0
	const DURATION := 0.4

	func _process(delta: float) -> void:
		_life += delta
		_r += 900.0 * delta
		if _life >= DURATION:
			queue_free()
			return
		queue_redraw()

	func _draw() -> void:
		var a := 1.0 - (_life / DURATION)
		var c := Color(color.r, color.g, color.b, a)
		draw_arc(Vector2.ZERO, _r, 0.0, TAU, 48, c, 4.0 * a + 1.0, true)
