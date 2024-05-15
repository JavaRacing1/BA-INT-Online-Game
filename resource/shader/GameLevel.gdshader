shader_type canvas_item;

uniform float blue = 1.0;

void vertex() {
	// Called for every vertex the material is visible on.
    VERTEX += vec2(cos(TIME)*100.0, sin(TIME)*100.0);
}

void fragment() {
	// Called for every pixel the material is visible on.
    COLOR = texture(TEXTURE, UV);
    COLOR.b = blue;
}
