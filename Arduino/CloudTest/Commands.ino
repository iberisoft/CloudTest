namespace ServerDebugCommand
{
	bool read(String command)
	{
		if (command == "SD=0")
		{
			serverDebug = false;
			return true;
		}
		if (command == "SD=1")
		{
			serverDebug = true;
			return true;
		}
		return false;
	}
}

namespace NetworkCommand
{
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

	bool read(String command)
	{
		if (command.startsWith("NW="))
		{
			int i = command.indexOf('+', 3);
			if (i >= 0)
			{
				setNetwork(command.substring(3, i), command.substring(i + 1));
				saveSettings();
			}
			return true;
		}
		return false;
	}
}

namespace SettingsCommand
{
	bool read(String command)
	{
		if (command == "SS?")
		{
			Serial.print("SS=");
			Serial.print(readSettings());
			Serial.print('\n');
			return true;
		}
		if (command.startsWith("SS="))
		{
			writeSettings(command.substring(3));
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
		NetworkCommand::read(command) ||
		SettingsCommand::read(command) ||
		RestartCommand::read(command);
}
