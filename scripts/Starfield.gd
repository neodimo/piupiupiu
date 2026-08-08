extends Node2D
class_name Starfield
## Deep-space layer behind the spring lattice. It follows the camera most of
## the way, making its stars drift slowly behind the foreground arena.

@export var parallax: float = 0.22
@export var field_half: Vector2 = Vector2(2800.0, 4300.0)
@export var star_count: int = 310

var _stars: Array[Dictionary] = []
var _twinkle_time := 0.0

func _ready() -> void:
	# A fixed seed makes the field feel like a place rather than TV static.
	var rng := RandomNumberGenerator.new()
	rng.seed = 732_041
	for i in star_count:
		var depth := rng.randf()
		_stars.append({
			"position": Vector2(rng.randf_range(-field_half.x, field_half.x), rng.randf_range(-field_half.y, field_half.y)),
			"radius": lerpf(0.8, 3.6, depth * depth),
			"phase": rng.randf_range(0.0, TAU),
			"color": _star_color(rng.randf(), depth),
		})
	queue_redraw()

func _process(delta: float) -> void:
	_twinkle_time += delta
	var camera := get_viewport().get_camera_2d()
	if camera:
		# A normal world layer travels at 1.0. Following the camera by this
		# amount leaves only `parallax` of the camera motion on screen.
		global_position = camera.global_position * (1.0 - parallax)
	queue_redraw()

func _star_color(tint: float, depth: float) -> Color:
	if tint < 0.18:
		return Color(0.7, 0.45, 1.25, lerpf(0.18, 0.62, depth))
	if tint < 0.42:
		return Color(0.35, 0.9, 1.45, lerpf(0.16, 0.58, depth))
	return Color(0.75, 0.88, 1.5, lerpf(0.13, 0.52, depth))

func _draw() -> void:
	for star in _stars:
		var pulse := 0.76 + sin(_twinkle_time * (1.2 + star.radius * 0.38) + star.phase) * 0.24
		var color: Color = star.color
		color.a *= pulse
		var radius: float = star.radius * pulse
		draw_circle(star.position, radius * 2.3, Color(color.r, color.g, color.b, color.a * 0.13))
		draw_circle(star.position, radius, color)
		if radius > 2.65:
			draw_line(star.position - Vector2(radius * 2.2, 0), star.position + Vector2(radius * 2.2, 0), Color(color.r, color.g, color.b, color.a * 0.45), 0.75, true)
			draw_line(star.position - Vector2(0, radius * 2.2), star.position + Vector2(0, radius * 2.2), Color(color.r, color.g, color.b, color.a * 0.45), 0.75, true)
