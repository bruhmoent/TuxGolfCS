extends Sprite2D

var time_passed: float = 0.0
var cycle_duration: float = 45.0 # seconds
var original_modulate: Color

func _ready():
	original_modulate = modulate

func _process(delta):
	time_passed += delta
	
	var cycle_time = fposmod(time_passed, cycle_duration)
	
	var normalized_time = sin(PI * (cycle_time / cycle_duration))
	
	var hue_shift = normalized_time * 0.1
	var brightness_shift = normalized_time * 0.5
	var saturation_shift = normalized_time * 0.5
	
	var new_modulate = original_modulate
	new_modulate.h = original_modulate.h + hue_shift
	new_modulate.s = original_modulate.s * (1.0 - saturation_shift)
	new_modulate.v = original_modulate.v * (1.0 - brightness_shift)
	
	modulate = new_modulate
