HX711 scale;

void setupScale()
{
	scale.begin(scaleDataPin, scaleClkPin);
	scale.set_scale(scaleCalibration);
}

float readScale()
{
	return scale.get_units(10);
}
