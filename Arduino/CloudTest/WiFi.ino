void setupWiFi()
{
	for (int i = 0; i < networks.size(); ++i)
	{
		if (setupWiFi(networks[i].Ssid, networks[i].Password))
		{
			return;
		}
	}

	WiFi.disconnect();
	Serial.println("Network not found");
}

bool setupWiFi(String ssid, String password)
{
	WiFi.begin(ssid.c_str(), password.c_str());
	Serial.print("Connecting to ");
	Serial.print(ssid);

	for (int i = 0; i < 20; ++i)
	{
		if (WiFi.status() == WL_CONNECTED)
		{
			Serial.println("");
			Serial.println("Network connected");
			Serial.println("IP address: ");
			Serial.println(WiFi.localIP());
			return true;
		}
		delay(500);
		Serial.print(".");
	}

	Serial.println("");
	Serial.println("Network failed");
	return false;
}
