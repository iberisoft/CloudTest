void setupButton()
{
	pinMode(buttonPin, INPUT);
}

bool isButtonPressed()
{
	return digitalRead(buttonPin) == HIGH;
}
