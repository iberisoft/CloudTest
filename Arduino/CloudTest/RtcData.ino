void loadRtcData()
{
	ESP.rtcUserMemoryRead(0, &rtcData.id, sizeof(rtcData));

	if (rtcData.id != 0x55aa)
	{
		rtcData.id = 0x55aa;
		rtcData.counter = 0;
	}
}

void saveRtcData()
{
	ESP.rtcUserMemoryWrite(0, &rtcData.id, sizeof(rtcData));
}
