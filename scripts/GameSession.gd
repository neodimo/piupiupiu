extends Node
## Global score / multiplier / run state.
## Mirrors the role of GameSession.cs in the original Unity build.

signal score_changed(score: int, multiplier: int)
signal game_over

const _SAVE_PATH := "user://hs.dat"
const MULT_DECAY_TIME := 4.5

var current_score: int = 0
var high_score: int = 0
var multiplier: int = 1
var is_running: bool = true

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
	score_changed.emit(current_score, multiplier)

func add_points(points: int) -> void:
	if not is_running:
		return
	current_score += points * multiplier
	if current_score > high_score:
		high_score = current_score
		_save_high_score()
	score_changed.emit(current_score, multiplier)

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
