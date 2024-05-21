shader_type canvas_item;

//Threshold for wave in percent
uniform float WAVE_THRESHOLD = 50.0;
//Speed of the wave
uniform float SPEED = 3.0;
//Interval of a wave
uniform float WAVE_INTERVAL = 30.0;

void fragment() {
    vec2 coord = UV;
    float alpha = 1.0;
    vec3 color = vec3(0.1, 0.3, 0.5);
    if (coord.y < WAVE_THRESHOLD / 100.0){
        float waveValue = sin(TIME * SPEED + coord.x * WAVE_INTERVAL) / 20.0 + 0.05;
        if (waveValue > coord.y){
            alpha = 0.0;
        } else {
            float distanceToWave = coord.y - waveValue;
            color.b = clamp(color.b + 0.3 - distanceToWave, 0.5, 1.0);
        }
    } 
    COLOR = vec4(color, alpha);
}