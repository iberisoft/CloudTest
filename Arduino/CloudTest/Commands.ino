namespace ServerDebugCommand
{
	bool read(String command)
	{
		if (command == "SD0")
		{
			serverDebug = false;
			return true;
		}
		if (command == "SD1")
		{
			serverDebug = true;
			return true;
		}
		return false;
	}
}

namespace ServerHostCommand
{
	void print()
	{
		Serial.print("SH=");
		Serial.print(serverHost);
		Serial.print('\n');
	}

	bool read(String command)
	{
		if (command == "SH?")
		{
			print();
			return true;
		}
		if (command.startsWith("SH="))
		{
			serverHost = command.substring(3);
			print();
			return true;
		}
		return false;
	}
}

namespace ServerPortCommand
{
	void print()
	{
		Serial.print("SP=");
		Serial.print(serverPort);
		Serial.print('\n');
	}

	bool read(String command)
	{
		if (command == "SP?")
		{
			print();
			return true;
		}
		if (command.startsWith("SP="))
		{
			serverPort = command.substring(3).toInt();
			print();
			return true;
		}
		return false;
	}
}

namespace TopicPrefixCommand
{
	void print()
	{
		Serial.print("TP=");
		Serial.print(topicPrefix);
		Serial.print('\n');
	}

	bool read(String command)
	{
		if (command == "TP?")
		{
			print();
			return true;
		}
		if (command.startsWith("TP="))
		{
			topicPrefix = command.substring(3);
			print();
			return true;
		}
		return false;
	}
}

namespace DeviceIdCommand
{
	void print()
	{
		Serial.print("ID=");
		Serial.print(deviceId);
		Serial.print('\n');
	}

	bool read(String command)
	{
		if (command == "ID?")
		{
			print();
			return true;
		}
		if (command.startsWith("ID="))
		{
			deviceId = command.substring(3);
			print();
			return true;
		}
		return false;
	}
}

namespace NetworksCommand
{
	void print()
	{
		Serial.print("NW=");
		for (int i = 0; i < networks.size(); ++i)
		{
			Serial.print(networks[i].Ssid);
			if (i < networks.size() - 1)
			{
				Serial.print('+');
			}
		}
		Serial.print('\n');
	}

	void setNetwork(String ssid, String password)
	{
		for (int i = 0; i < networks.size(); ++i)
		{
			if (networks[i].Ssid == ssid)
			{
				networks[i].Password = password;
				return;
			}
		}

		Network network;
		network.Ssid = ssid;
		network.Password = password;
		networks.add(network);
	}

	void clearNetwork(String ssid)
	{
		for (int i = 0; i < networks.size(); ++i)
		{
			if (networks[i].Ssid == ssid)
			{
				networks.remove(i);
				return;
			}
		}
	}

	bool read(String command)
	{
		if (command == "NW?")
		{
			print();
			return true;
		}
		if (command.startsWith("NW="))
		{
			int i = command.indexOf('+', 3);
			if (i >= 0)
			{
				setNetwork(command.substring(3, i), command.substring(i + 1));
			}
			else
			{
				clearNetwork(command.substring(3));
			}
			print();
			return true;
		}
		return false;
	}
}

namespace SaveSettingsCommand
{
	bool read(String command)
	{
		if (command == "SS")
		{
			saveSettings();
			return true;
		}
		return false;
	}
}

namespace RestartCommand
{
	bool read(String command)
	{
		if (command == "RST")
		{
			ESP.restart();
			return true;
		}
		return false;
	}
}

bool readCommand()
{
	String command = Serial.readStringUntil('\n');
	return ServerDebugCommand::read(command) ||
		ServerHostCommand::read(command) ||
		ServerPortCommand::read(command) ||
		TopicPrefixCommand::read(command) ||
		DeviceIdCommand::read(command) ||
		NetworksCommand::read(command) ||
		SaveSettingsCommand::read(command) ||
		RestartCommand::read(command);
}
