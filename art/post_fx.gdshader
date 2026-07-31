shader_type canvas_item;
// Full-screen post pass layered over the glow: edge chromatic aberration,
// vignette, and a faint animated grain. Neon-arena screen feel.

uniform sampler2D screen_tex : hint_screen_texture, filter_linear_mipmap;
uniform float aberration : hint_range(0.0, 6.0) = 2.0;
uniform float vignette_strength : hint_range(0.0, 1.0) = 0.55;
uniform float grain : hint_range(0.0, 0.2) = 0.035;
uniform float saturation : hint_range(0.0, 2.0) = 1.0;
uniform float contrast : hint_range(0.5, 1.8) = 1.0;
uniform float brightness : hint_range(-0.25, 0.25) = 0.0;
uniform float lens_distortion : hint_range(-0.35, 0.35) = 0.0;

void fragment() {
	vec2 uv = SCREEN_UV;
	vec2 dir = uv - vec2(0.5);
	float dist = length(dir);
	// Radial lens control: positive values barrel-distort, negative values
	// pincushion-distort. Keep the edge sampling stable when pushed hard.
	float r2 = dot(dir, dir);
	vec2 lens_uv = vec2(0.5) + dir * (1.0 + lens_distortion * r2 * 3.0);
	lens_uv = clamp(lens_uv, vec2(0.001), vec2(0.999));
	// chromatic aberration grows toward the edges
	vec2 off = dir * dist * aberration * 0.012;
	vec3 col;
	col.r = texture(screen_tex, lens_uv + off).r;
	col.g = texture(screen_tex, lens_uv).g;
	col.b = texture(screen_tex, lens_uv - off).b;
	// Simple filmic color grade controls that behave predictably on device.
	float luminance = dot(col, vec3(0.2126, 0.7152, 0.0722));
	col = mix(vec3(luminance), col, saturation);
	col = (col - 0.5) * contrast + 0.5 + brightness;
	// vignette
	float v = smoothstep(0.85, 0.25, dist);
	col *= mix(1.0, v, vignette_strength);
	// animated grain
	float n = fract(sin(dot(uv * (TIME * 60.0 + 1.0), vec2(12.9898, 78.233))) * 43758.5453);
	col += (n - 0.5) * grain;
	COLOR = vec4(col, 1.0);
}
