String serverHost = "";
int serverPort = 1883;
String topicPrefix = "cloud/test";
String deviceId = "00000";

struct Network
{
	String Ssid;
	String Password;
};

LinkedList<Network> networks;

struct
{
	uint32_t id;
	uint32_t counter;
} rtcData;
