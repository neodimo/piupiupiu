extends Control
## Settings page — music/sfx volume + haptics toggle, persisted via Settings.
## Reached from the title screen; Back returns to the title.

@onready var _music: HSlider = $Panel/VBox/MusicRow/MusicSlider
@onready var _sfx: HSlider = $Panel/VBox/SfxRow/SfxSlider
@onready var _haptics: CheckButton = $Panel/VBox/HapticsRow/HapticsCheck
@onready var _preview: AudioStreamPlayer = $Preview

func _ready() -> void:
	_music.value = Settings.music_volume
	_sfx.value = Settings.sfx_volume
	_haptics.button_pressed = Settings.haptics
	_music.value_changed.connect(_on_music)
	_sfx.value_changed.connect(_on_sfx)
	_haptics.toggled.connect(_on_haptics)
	$Panel/VBox/BackBtn.pressed.connect(_on_back)
	_preview.stream = load("res://art/venus.wav")
	_preview.volume_db = Settings.music_db()
	_preview.play()

func _on_music(v: float) -> void:
	Settings.music_volume = v
	_preview.volume_db = Settings.music_db()

func _on_sfx(v: float) -> void:
	Settings.sfx_volume = v

func _on_haptics(on: bool) -> void:
	Settings.haptics = on
	Settings.buzz(20)   # feedback when enabling

func _on_back() -> void:
	Settings.save_settings()
	get_tree().change_scene_to_file("res://scenes/TitleScreen.tscn")
