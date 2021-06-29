#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include <FS.h>
#include <LinkedList.h>
#include "DeviceConfig.h"
#include "Settings.h"
#include "Server.h"

void setup()
{
	Serial.begin(9600);
	Serial.println();

	loadSettings();
	if (setupWiFi())
	{
		saveSettings();
	}
	setupServer(receiveData);
}

uint32_t updateTime = 0;

void loop()
{
	switch (connectServer())
	{
	case 1:
		// subscribe
	case 2:
		pollServer();
		if (millis() - updateTime > deviceIdle)
		{
			updateTime = millis();
			heartbeat();
		}
		break;
	}

	if (Serial.available())
	{
		readCommand();
	}
}

uint32_t counter = 0;

void heartbeat()
{
	publishData("heartbeat", "counter", counter++);
}

void receiveData(String topic, String data)
{
}
