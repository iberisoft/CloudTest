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
		SettingsCommand::read(command) ||
		RestartCommand::read(command);
}
