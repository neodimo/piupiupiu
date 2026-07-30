class_name SheetAnim
## Builds a looping SpriteFrames from a horizontal sprite strip.
## The Unity art ships as single-row strips of square frames.

static func build(sheet: Texture2D, frame_size: int, fps: float = 18.0) -> SpriteFrames:
	var frames := SpriteFrames.new()
	frames.set_animation_speed("default", fps)
	frames.set_animation_loop("default", true)
	var count := int(sheet.get_width() / frame_size)
	# frame 0 of these sheets is often near-empty; keep all frames, playback hides it
	for i in count:
		var atlas := AtlasTexture.new()
		atlas.atlas = sheet
		atlas.region = Rect2(i * frame_size, 0, frame_size, sheet.get_height())
		frames.add_frame("default", atlas)
	return frames

## Builds a looping SpriteFrames from a list of full-image textures (one per
## frame). Used for enemies whose art ships as individual sprite files rather
## than a packed strip.
static func build_from_textures(textures: Array, fps: float = 6.0) -> SpriteFrames:
	var frames := SpriteFrames.new()
	frames.set_animation_speed("default", fps)
	frames.set_animation_loop("default", true)
	for tex in textures:
		frames.add_frame("default", tex)
	return frames
