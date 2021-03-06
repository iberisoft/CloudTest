#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include <FS.h>
#include <LinkedList.h>
#include <HX711.h>
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
	setupScale();
	setupButton();
	setupServer(receiveData);

	if (WiFi.status() != WL_CONNECTED)
	{
		deepSleep(deviceIdle);
	}
}

uint32_t updateTime = -deviceIdle * 1000;

void loop()
{
	switch (connectServer())
	{
	case 1:
		subscribeData("sleep");
	case 2:
		pollServer();
		if (millis() - updateTime > deviceIdle * 1000)
		{
			updateTime = millis();
			heartbeat();
			updateScale();
			updateButton();
			updateWiFi();
		}
		break;
	}

	if (Serial.available())
	{
		readCommand();
	}

	delay(1);
}

void heartbeat()
{
	loadRtcData();
	publishData("heartbeat", "counter", rtcData.counter++);
	saveRtcData();
}

void updateScale()
{
	if (hasScale())
	{
		publishData("scale", "weight", readScale());
	}
}

void updateButton()
{
	publishData("button", "state", isButtonPressed());
}

void updateWiFi()
{
	StaticJsonDocument<1024> doc;
	JsonArray networksJson = doc.createNestedArray("networks");
	scanWiFi(networksJson);
	publishData("wifi", doc);
}

void receiveData(String topic, String data)
{
	if (topic == "sleep")
	{
		StaticJsonDocument<256> doc;
		deserializeJson(doc, data);
		int sec = doc["sec"];

		delay(1000);
		disconnectServer();
		deepSleep(sec);
	}
}

void deepSleep(int sec)
{
	Serial.print("Deep sleep for ");
	Serial.print(sec);
	Serial.println(" sec...");
	ESP.deepSleep(sec * 1000000);
}
