#include <WiFiManager.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include <FS.h>
#include "DeviceConfig.h"
#include "Settings.h"
#include "Server.h"

void setup()
{
	Serial.begin(9600);
	Serial.println();

	setupWiFi();
	setupServer(receiveData);
}

void loop()
{
	switch (connectServer())
	{
	case 1:
		// subscribe
	case 2:
		pollServer();
		heartbeat();
		break;
	}

	if (Serial.available())
	{
		readCommand();
	}

	delay(deviceIdle);
}

void readCommand()
{
	String command = Serial.readStringUntil('\n');
	if (command == "ID")
	{
		Serial.print(deviceId);
		Serial.print('\n');
		return;
	}
	if (command == "RST")
	{
		resetWiFi();
		return;
	}
}

void heartbeat()
{
	StaticJsonDocument<256> doc;
	doc["ms"] = millis();
	String data;
	serializeJson(doc, data);
	publishData("heartbeat", data);
}

void receiveData(String topic, String data)
{
}
