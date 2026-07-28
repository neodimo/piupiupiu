extends Node2D
## Mass-spring vector grid — the Geometry Wars "flex" background.
## Godot-native rebuild of the Unity VectorGrid asset: a lattice of point
## masses connected by springs to their neighbours and their rest position,
## disturbed by explosions and the player, drawn as glowing lines.

@export var cols: int = 22
@export var rows: int = 38
@export var spacing: float = 50.0
@export var stiffness: float = 28.0      # pull back toward rest
@export var link_stiffness: float = 12.0 # pull toward neighbours
@export var damping: float = 4.0
@export var line_color: Color = Color(0.15, 0.35, 0.85, 0.55)

var _rest: PackedVector2Array = PackedVector2Array()
var _pos: PackedVector2Array = PackedVector2Array()
var _vel: PackedVector2Array = PackedVector2Array()

func _idx(c: int, r: int) -> int:
	return r * cols + c

func _ready() -> void:
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
	queue_redraw()

func _draw() -> void:
	for r in rows:
		for c in cols:
			var i := _idx(c, r)
			if c + 1 < cols:
				draw_line(_pos[i], _pos[_idx(c + 1, r)], line_color, 1.5, true)
			if r + 1 < rows:
				draw_line(_pos[i], _pos[_idx(c, r + 1)], line_color, 1.5, true)
