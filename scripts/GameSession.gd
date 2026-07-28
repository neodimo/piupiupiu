extends Node
## Global score / multiplier / run state.
## Mirrors the role of GameSession.cs in the original Unity build.

signal score_changed(score: int, multiplier: int)
signal game_over

var current_score: int = 0
var high_score: int = 0
var multiplier: int = 1
var is_running: bool = true

func reset() -> void:
	current_score = 0
	multiplier = 1
	is_running = true
	score_changed.emit(current_score, multiplier)

func add_points(points: int) -> void:
	if not is_running:
		return
	current_score += points * multiplier
	if current_score > high_score:
		high_score = current_score
	score_changed.emit(current_score, multiplier)

func bump_multiplier() -> void:
	multiplier = min(multiplier + 1, 99)
	score_changed.emit(current_score, multiplier)

func end_run() -> void:
	if not is_running:
		return
	is_running = false
	game_over.emit()
