shader_type canvas_item;

//Threshold for wave in percent
uniform float WAVE_THRESHOLD = 50.0;
//Speed of the wave
uniform float SPEED = 3.0;
//Size of a wave
uniform float WAVE_SIZE = 30.0;

void fragment() {
    vec2 coord = UV;
    float alpha = 1.0;
    if (coord.y < WAVE_THRESHOLD / 100.0){
        float waveValue = sin(TIME * SPEED + coord.x * WAVE_SIZE) / 5.0 + 0.2;
        if (waveValue > coord.y){
            alpha = 0.0;
        }
    } 
    COLOR = vec4(0.1, 0.4, 0.6, alpha);
}