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
		}
		break;
	}

	if (Serial.available())
	{
		readCommand();
	}

	if (rtcData.counter > deepSleepStartTime / deviceIdle)
	{
		delay(10);
		ESP.deepSleep(deviceIdle * 1000);
	}
}

void heartbeat()
{
	loadRtcData();
	publishData("heartbeat", "counter", rtcData.counter++);
	saveRtcData();
}

void receiveData(String topic, String data)
{
}
