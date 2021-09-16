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
	setupServer(receiveData);

	if (WiFi.status() != WL_CONNECTED)
	{
		ESP.deepSleep(deviceIdle * 1000);
	}
}

uint32_t updateTime = -deviceIdle;

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
			updateScale();
			updateWiFi();
		}
		break;
	}

	if (Serial.available())
	{
		readCommand();
	}

	if (rtcData.counter > deepSleepStartTime / deviceIdle)
	{
		delay(1000);
		disconnectServer();
		ESP.deepSleep(deviceIdle * 1000);
	}
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

void updateWiFi()
{
	StaticJsonDocument<1024> doc;
	JsonArray networksJson = doc.createNestedArray("networks");
	scanWiFi(networksJson);
	publishData("wifi", doc);
}

void receiveData(String topic, String data)
{
}
