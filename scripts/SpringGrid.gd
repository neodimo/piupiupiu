extends Node2D
## Mass-spring vector grid — the Geometry Wars "flex" background.
## Godot-native rebuild of the Unity VectorGrid asset: a lattice of point
## masses connected by springs to their neighbours and their rest position,
## disturbed by explosions and the player, drawn as glowing lines whose
## colour shifts with local displacement/energy (blue at rest → cyan → green
## → hot yellow where the grid is violently pushed). Colours are pushed into
## HDR range so the WorldEnvironment glow blooms the energetic lines.

## The grid is an arena-sized world surface. It intentionally extends beyond
## the portrait viewport so camera travel reveals new grid space.
@export var cols: int = 40
@export var rows: int = 72
@export var spacing: float = 50.0
@export var arena_half: Vector2 = Vector2(950.0, 1750.0)
@export var stiffness: float = 28.0      # pull back toward rest
@export var link_stiffness: float = 12.0 # pull toward neighbours
@export var damping: float = 4.0
## Displacement (px) at which a line reaches full hot colour.
@export var color_energy_scale: float = 90.0
@export var rest_color: Color = Color(0.12, 0.28, 0.75, 0.45)

var _rest: PackedVector2Array = PackedVector2Array()
var _pos: PackedVector2Array = PackedVector2Array()
var _vel: PackedVector2Array = PackedVector2Array()
var _hue_time: float = 0.0   # drives the psychedelic hue cycling

func _idx(c: int, r: int) -> int:
	return r * cols + c

func _ready() -> void:
	# Keep the declared arena and lattice in agreement if either is tuned.
	cols = maxi(cols, int(ceil(arena_half.x * 2.0 / spacing)) + 1)
	rows = maxi(rows, int(ceil(arena_half.y * 2.0 / spacing)) + 1)
	var origin := Vector2(-(cols - 1) * spacing * 0.5, -(rows - 1) * spacing * 0.5)
	for r in rows:
		for c in cols:
			var p := origin + Vector2(c * spacing, r * spacing)
			_rest.append(p)
			_pos.append(p)
			_vel.append(Vector2.ZERO)

## Push the grid outward from a world point — call on explosions / impacts.
func disturb(world_pos: Vector2, strength: float, radius: float = 260.0) -> void:
	var local := to_local(world_pos)
	for i in _pos.size():
		var d := _pos[i] - local
		var dist := d.length()
		if dist < radius and dist > 0.001:
			var falloff := 1.0 - dist / radius
			_vel[i] += d.normalized() * strength * falloff

func _physics_process(delta: float) -> void:
	var n := _pos.size()
	for i in n:
		var force := (_rest[i] - _pos[i]) * stiffness
		force -= _vel[i] * damping
		_vel[i] += force * delta
	# neighbour springs (right + down), applied symmetrically
	for r in rows:
		for c in cols:
			var i := _idx(c, r)
			if c + 1 < cols:
				var j := _idx(c + 1, r)
				var f: Vector2 = (_pos[j] - _pos[i]) - (_rest[j] - _rest[i])
				f *= link_stiffness * delta
				_vel[i] += f
				_vel[j] -= f
			if r + 1 < rows:
				var j2 := _idx(c, r + 1)
				var f2: Vector2 = (_pos[j2] - _pos[i]) - (_rest[j2] - _rest[i])
				f2 *= link_stiffness * delta
				_vel[i] += f2
				_vel[j2] -= f2
	for i in n:
		_pos[i] += _vel[i] * delta
	_hue_time += delta
	queue_redraw()

## Displacement magnitude of a grid point from its rest position.
func _disp(i: int) -> float:
	return (_pos[i] - _rest[i]).length()

## Map a line's energy (avg endpoint displacement) to a glowing HDR colour.
## Rest → dim blue; energetic → saturated cyan/green/yellow boosted past 1.0
## so bloom catches it. This is the "colourful and amazing" grid feel.
func _line_color(energy: float, mid: Vector2) -> Color:
	var e := clampf(energy / color_energy_scale, 0.0, 1.0)
	if e <= 0.001:
		return rest_color
	# Hue sweeps the FULL colour wheel over time + space (not tied to energy),
	# so disturbances shimmer through the whole rainbow — psychedelic, not green.
	var hue := fposmod(_hue_time * 0.35 + mid.length() * 0.0017 + mid.x * 0.0007, 1.0)
	var c := Color.from_hsv(hue, 0.92, 1.0)
	var boost := 1.0 + e * e * 2.8
	c.r *= boost
	c.g *= boost
	c.b *= boost
	c.a = lerpf(rest_color.a, 1.0, e)
	return c

func _draw() -> void:
	for r in rows:
		for c in cols:
			var i := _idx(c, r)
			if c + 1 < cols:
				var j := _idx(c + 1, r)
				var e := (_disp(i) + _disp(j)) * 0.5
				var mid := (_pos[i] + _pos[j]) * 0.5
				var col := _line_color(e, mid)
				var w := 1.5 + clampf(e / color_energy_scale, 0.0, 1.0) * 1.5
				draw_line(_pos[i], _pos[j], col, w, true)
			if r + 1 < rows:
				var j2 := _idx(c, r + 1)
				var e2 := (_disp(i) + _disp(j2)) * 0.5
				var mid2 := (_pos[i] + _pos[j2]) * 0.5
				var col2 := _line_color(e2, mid2)
				var w2 := 1.5 + clampf(e2 / color_energy_scale, 0.0, 1.0) * 1.5
				draw_line(_pos[i], _pos[j2], col2, w2, true)
