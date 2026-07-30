extends Node2D
class_name Main
## Root bootstrap: camera, grid, player, spawner, HUD wiring.

static var instance: Main

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
	_start_music()
	if OS.has_environment("PIU_CAPTURE"):
		_capture_after(float(OS.get_environment("PIU_CAPTURE")))

func _start_music() -> void:
	_bgm = AudioStreamPlayer.new()
	_bgm.stream = load("res://art/venus.wav")
	_bgm.volume_db = -8.0
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
	Input.vibrate_handheld(120)
	get_tree().create_timer(3.0).timeout.connect(func(): get_tree().reload_current_scene())

## Floating "+N" score popup at a kill — drifts up and fades (PopupScoreMovement.cs).
static func spawn_score_popup(pos: Vector2, points: int, color: Color = Color(1.6, 1.8, 1.0)) -> void:
	if instance == null:
		return
	var lbl := Label.new()
	lbl.text = "+%d" % points
	lbl.add_theme_font_size_override("font_size", 44)
	lbl.add_theme_color_override("font_color", color)
	lbl.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	lbl.z_index = 50
	instance.add_child(lbl)
	lbl.global_position = pos + Vector2(-40, -30)
	var t := lbl.create_tween()
	t.set_parallel(true)
	t.tween_property(lbl, "global_position:y", lbl.global_position.y - 130.0, 0.7)
	t.tween_property(lbl, "modulate:a", 0.0, 0.7).set_delay(0.15)
	t.chain().tween_callback(lbl.queue_free)

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
