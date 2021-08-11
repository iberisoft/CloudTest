HX711 scale;

void setupScale()
{
	scale.begin(scaleDataPin, scaleClkPin);
	scale.set_scale(scaleCalibration);
}

bool hasScale()
{
	return scale.is_ready();
}

float readScale()
{
	return scale.get_units(10);
}
