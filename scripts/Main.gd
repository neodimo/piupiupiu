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
	if OS.has_environment("PIU_CAPTURE"):
		_capture_after(float(OS.get_environment("PIU_CAPTURE")))

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

## Per-enemy death VFX: a coloured star-burst (explosion_stars art) plus an
## expanding shockwave ring. Tinted to the enemy so kills read distinctly.
static func spawn_death_vfx(pos: Vector2, color: Color = Color(0.5, 0.9, 1.0)) -> void:
	if instance == null:
		return
	# star-burst particles using the real explosion art (3 frames of 30x32)
	var tex := load("res://art/explosion_stars.png") as Texture2D
	var p := CPUParticles2D.new()
	instance.add_child(p)
	p.global_position = pos
	p.texture = tex
	p.emitting = true
	p.one_shot = true
	p.explosiveness = 1.0
	p.amount = 20
	p.lifetime = 0.55
	p.initial_velocity_min = 240.0
	p.initial_velocity_max = 600.0
	p.spread = 180.0
	p.angular_velocity_min = -400.0
	p.angular_velocity_max = 400.0
	p.scale_amount_min = 1.4
	p.scale_amount_max = 3.2
	p.color = color
	var g := Gradient.new()
	g.set_color(0, color)
	g.set_color(1, Color(color.r, color.g, color.b, 0.0))
	p.color_ramp = g
	p.get_tree().create_timer(0.9).timeout.connect(p.queue_free)
	# expanding shockwave ring
	var ring := _Shockwave.new()
	ring.color = color
	instance.add_child(ring)
	ring.global_position = pos

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
