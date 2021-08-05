void publishData(String topic, String name, uint32_t value)
{
	StaticJsonDocument<256> doc;
	doc[name] = value;
	publishData(topic, doc);
}

void publishData(String topic, JsonDocument& doc)
{
	String data;
	serializeJson(doc, data);
	publishData(topic, data);
}
