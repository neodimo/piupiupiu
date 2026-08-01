extends Node
## Global persistent settings (audio, haptics, post-processing, god mode).
## Autoloaded. Mirrors the role of the Unity SaveGame/settings for the rewrite.

const _PATH := "user://settings.cfg"

var music_volume: float = 0.8   # 0..1
var sfx_volume: float = 0.9     # 0..1
var haptics: bool = true

# Post-processing — dialable to find a sweet spot. Defaults match the tuned look.
var fx_glow: float = 0.9        # glow intensity (0..3)
var fx_bloom: float = 0.0       # global bloom / lift (0..0.6) — raises blacks, use sparingly
var fx_aberration: float = 1.3  # chromatic aberration (0..5)
var fx_vignette: float = 0.55   # edge darkening (0..1)
var fx_grain: float = 0.012     # film grain (0..0.08)
var fx_saturation: float = 1.0  # color grade saturation (0..2)
var fx_contrast: float = 1.0    # color grade contrast (0.5..1.8)
var fx_brightness: float = 0.0  # color grade lift (-0.25..0.25)
var fx_lens_distortion: float = 0.0 # barrel/pincushion strength (-0.35..0.35)
var player_distortion: float = 16.0 # player movement distortion strength (4..35)

# Enemy filters — useful for parity/debug runs and for tuning one behavior at a time.
var enemy_basic_enabled: bool = true
var enemy_wave_enabled: bool = true
var enemy_smart_enabled: bool = true
var enemy_boss_enabled: bool = true

var god_mode: bool = false      # player can't die — for testing mechanics

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
	cfg.set_value("fx", "glow", fx_glow)
	cfg.set_value("fx", "bloom", fx_bloom)
	cfg.set_value("fx", "aberration", fx_aberration)
	cfg.set_value("fx", "vignette", fx_vignette)
	cfg.set_value("fx", "grain", fx_grain)
	cfg.set_value("fx", "saturation", fx_saturation)
	cfg.set_value("fx", "contrast", fx_contrast)
	cfg.set_value("fx", "brightness", fx_brightness)
	cfg.set_value("fx", "lens_distortion", fx_lens_distortion)
	cfg.set_value("gameplay", "player_distortion", player_distortion)
	cfg.set_value("debug", "god_mode", god_mode)
	cfg.set_value("enemies", "basic", enemy_basic_enabled)
	cfg.set_value("enemies", "wave", enemy_wave_enabled)
	cfg.set_value("enemies", "smart", enemy_smart_enabled)
	cfg.set_value("enemies", "boss", enemy_boss_enabled)
	cfg.save(_PATH)

func load_settings() -> void:
	var cfg := ConfigFile.new()
	if cfg.load(_PATH) != OK:
		return
	music_volume = cfg.get_value("audio", "music", music_volume)
	sfx_volume = cfg.get_value("audio", "sfx", sfx_volume)
	haptics = cfg.get_value("input", "haptics", haptics)
	fx_glow = cfg.get_value("fx", "glow", fx_glow)
	fx_bloom = cfg.get_value("fx", "bloom", fx_bloom)
	fx_aberration = cfg.get_value("fx", "aberration", fx_aberration)
	fx_vignette = cfg.get_value("fx", "vignette", fx_vignette)
	fx_grain = cfg.get_value("fx", "grain", fx_grain)
	fx_saturation = cfg.get_value("fx", "saturation", fx_saturation)
	fx_contrast = cfg.get_value("fx", "contrast", fx_contrast)
	fx_brightness = cfg.get_value("fx", "brightness", fx_brightness)
	fx_lens_distortion = cfg.get_value("fx", "lens_distortion", fx_lens_distortion)
	player_distortion = cfg.get_value("gameplay", "player_distortion", player_distortion)
	god_mode = cfg.get_value("debug", "god_mode", god_mode)
	enemy_basic_enabled = cfg.get_value("enemies", "basic", enemy_basic_enabled)
	enemy_wave_enabled = cfg.get_value("enemies", "wave", enemy_wave_enabled)
	enemy_smart_enabled = cfg.get_value("enemies", "smart", enemy_smart_enabled)
	enemy_boss_enabled = cfg.get_value("enemies", "boss", enemy_boss_enabled)
