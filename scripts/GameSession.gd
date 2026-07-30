extends Node
## Global score / multiplier / run state.
## Mirrors the role of GameSession.cs in the original Unity build.

signal score_changed(score: int, multiplier: int)
signal game_over
signal exp_changed(current_exp: int, required_exp: int, level: int)
signal leveled_up(level: int)

const _SAVE_PATH := "user://hs.dat"
const MULT_DECAY_TIME := 4.5

var current_score: int = 0
var high_score: int = 0
var multiplier: int = 1
var is_running: bool = true

var exp: int = 0
var level: int = 0
var _exp_required: int = 8

var _mult_timer: float = 0.0

func _ready() -> void:
	_load_high_score()

func _process(delta: float) -> void:
	if not is_running or multiplier <= 1:
		return
	_mult_timer += delta
	if _mult_timer >= MULT_DECAY_TIME:
		_mult_timer = 0.0
		multiplier = max(1, multiplier - 1)
		score_changed.emit(current_score, multiplier)

func reset() -> void:
	current_score = 0
	multiplier = 1
	is_running = true
	_mult_timer = 0.0
	exp = 0
	level = 0
	_exp_required = 8
	score_changed.emit(current_score, multiplier)
	exp_changed.emit(0, _exp_required, 0)

func add_points(points: int) -> void:
	if not is_running:
		return
	current_score += points * multiplier
	if current_score > high_score:
		high_score = current_score
		_save_high_score()
	score_changed.emit(current_score, multiplier)
	_add_exp(1)

func _add_exp(amount: int) -> void:
	exp += amount
	if exp >= _exp_required:
		exp -= _exp_required
		level += 1
		_exp_required = 8 + level * 4
		leveled_up.emit(level)
	exp_changed.emit(exp, _exp_required, level)

func bump_multiplier() -> void:
	_mult_timer = 0.0
	multiplier = min(multiplier + 1, 99)
	score_changed.emit(current_score, multiplier)

func end_run() -> void:
	if not is_running:
		return
	is_running = false
	game_over.emit()

func _save_high_score() -> void:
	var f := FileAccess.open(_SAVE_PATH, FileAccess.WRITE)
	if f:
		f.store_64(high_score)

func _load_high_score() -> void:
	var f := FileAccess.open(_SAVE_PATH, FileAccess.READ)
	if f:
		high_score = f.get_64()
