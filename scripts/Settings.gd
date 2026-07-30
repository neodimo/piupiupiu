extends Node
## Global persistent settings (music/sfx volume + haptics). Autoloaded.
## Mirrors the role of the Unity SaveGame/settings for the rewrite.

const _PATH := "user://settings.cfg"

var music_volume: float = 0.8   # 0..1
var sfx_volume: float = 0.9     # 0..1
var haptics: bool = true

func _ready() -> void:
	load_settings()

## Central haptics gate — all game buzzes route through here so the toggle works.
func buzz(ms: int) -> void:
	if haptics:
		Input.vibrate_handheld(ms)

func music_db() -> float:
	if music_volume <= 0.001:
		return -80.0
	return linear_to_db(music_volume)

func save_settings() -> void:
	var cfg := ConfigFile.new()
	cfg.set_value("audio", "music", music_volume)
	cfg.set_value("audio", "sfx", sfx_volume)
	cfg.set_value("input", "haptics", haptics)
	cfg.save(_PATH)

func load_settings() -> void:
	var cfg := ConfigFile.new()
	if cfg.load(_PATH) != OK:
		return
	music_volume = cfg.get_value("audio", "music", music_volume)
	sfx_volume = cfg.get_value("audio", "sfx", sfx_volume)
	haptics = cfg.get_value("input", "haptics", haptics)
