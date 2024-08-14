shader_type canvas_item;

uniform float flash_speed : hint_range(0.1, 10.0) = 1.0;

void fragment() {
    float flash_factor = 0.5 + 0.5 * sin(TIME * flash_speed);
    
    vec3 color_orange = vec3(1.0, 0.5, 0.0);
    vec3 color_white = vec3(1.0, 1.0, 1.0);
    
    vec3 flash_color = mix(color_orange, color_white, flash_factor);
    
    vec4 texture_color = texture(TEXTURE, UV);
    COLOR = texture_color * vec4(flash_color, 1.0);
}