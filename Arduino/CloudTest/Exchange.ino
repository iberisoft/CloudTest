void publishData(String topic, String name, uint32_t value)
{
	StaticJsonDocument<256> doc;
	doc[name] = value;
	String data;
	serializeJson(doc, data);
	publishData(topic, data);
}
