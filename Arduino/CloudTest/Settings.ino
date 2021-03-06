const char* settingsFilePath = "/settings.json";

void loadSettings()
{
	if (SPIFFS.begin())
	{
		Serial.println("Filesystem mounted");
	}
	else
	{
		Serial.println("Filesystem failed to mount");
		return;
	}

	if (SPIFFS.exists(settingsFilePath))
	{
		StaticJsonDocument<1024> doc;

		File file = SPIFFS.open(settingsFilePath, "r");
		deserializeJson(doc, file);
		file.close();
		Serial.println("Loading config file");

		serverHost = (const char*)doc["serverHost"];
		serverPort = doc["serverPort"];
		topicPrefix = (const char*)doc["topicPrefix"];
		deviceId = (const char*)doc["deviceId"];

		networks.clear();
		JsonArray networksJson = doc["networks"].as<JsonArray>();
		for (int i = 0; i < networksJson.size(); ++i)
		{
			Network network;
			network.Ssid = (const char*)networksJson[i]["ssid"];
			network.Password = (const char*)networksJson[i]["password"];
			networks.add(network);
		}
	}
	else
	{
		Serial.println("Config file not found");
	}
}

void saveSettings()
{
	StaticJsonDocument<1024> doc;

	doc["serverHost"] = serverHost;
	doc["serverPort"] = serverPort;
	doc["topicPrefix"] = topicPrefix;
	doc["deviceId"] = deviceId;

	JsonArray networksJson = doc.createNestedArray("networks");
	for (int i = 0; i < networks.size(); ++i)
	{
		networksJson[i]["ssid"] = networks[i].Ssid;
		networksJson[i]["password"] = networks[i].Password;
	}

	File file = SPIFFS.open(settingsFilePath, "w");
	serializeJson(doc, file);
	file.close();
	Serial.println("Saving config file");
}

String readSettings()
{
	if (!SPIFFS.exists(settingsFilePath))
	{
		saveSettings();
	}

	File file = SPIFFS.open(settingsFilePath, "r");
	String text;
	while (file.available())
	{
		text += (char)file.read();
	}
	file.close();
	return text;
}

void writeSettings(String text)
{
	File file = SPIFFS.open(settingsFilePath, "w");
	file.print(text);
	file.close();
}
