shader_type canvas_item;
// Out-of-focus darkened blur of whatever is rendered below — used to push the
// live background demo behind the title UI.

uniform sampler2D screen_tex : hint_screen_texture, filter_linear_mipmap;
uniform float blur : hint_range(0.0, 6.0) = 2.2;
uniform float dim : hint_range(0.0, 1.0) = 0.5;

void fragment() {
	vec2 ps = SCREEN_PIXEL_SIZE * blur;
	vec3 c = vec3(0.0);
	for (int x = -2; x <= 2; x++) {
		for (int y = -2; y <= 2; y++) {
			c += texture(screen_tex, SCREEN_UV + vec2(float(x), float(y)) * ps).rgb;
		}
	}
	c /= 25.0;
	COLOR = vec4(c * dim, 1.0);
}
