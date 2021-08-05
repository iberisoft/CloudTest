bool setupWiFi()
{
	for (int i = 0; i < networks.size(); ++i)
	{
		if (setupWiFi(networks[i].Ssid, networks[i].Password))
		{
			if (i > 0)
			{
				networks.unshift(networks.remove(i));
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	WiFi.disconnect();
	Serial.println("Network not found");
	return false;
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

void scanWiFi(JsonDocument& doc)
{
	int count = WiFi.scanNetworks();
	for (int i = 0; i < count; ++i)
	{
		doc[i]["ssid"] = WiFi.SSID(i);
		doc[i]["rssi"] = WiFi.RSSI(i);
		delay(10);
	}
}
