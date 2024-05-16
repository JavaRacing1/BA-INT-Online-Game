shader_type canvas_item;

uniform sampler2D screen_texture: hint_screen_texture, filter_linear_mipmap;

void fragment() {
    COLOR.rgb = textureLod(screen_texture, SCREEN_UV, 2).rgb;
    
    float alpha = 1.0;
    float average = (COLOR.r + COLOR.g + COLOR.b) / 3.0;
    vec3 color = vec3(0.0, 0.0, 0.0);
    
    if (average < 0.2){
        alpha = 0.0;
    } else if (average < 0.4){
        color.r = 0.3;
        color.g = 0.6;
        color.b = 0.8;
    } else {
        color.r = 0.2;
        color.g = 0.8;
        color.b = 1.0;
    }
    COLOR = vec4(color, alpha);
    
    /*
    float alpha = 1.0;
    vec3 color = texture(TEXTURE, UV).rgb;
    float average = (COLOR.r + COLOR.g + COLOR.b) / 3.0;
    if (average < 0.2){
        alpha = 0.0;
    } else if (average < 0.4){
        color.r = 0.3;
        color.g = 0.6;
        color.b = 0.8;
    } else {
        color.r = 0.2;
        color.g = 0.8;
        color.b = 1.0;
    }
    COLOR = vec4(color, alpha);
    */
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
