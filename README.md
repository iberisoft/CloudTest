# Cloud Test

## Server

### Run on Ubuntu

Update Ubuntu:

```
apt update
apt upgrade
```

Install Docker:

```
apt install docker.io
```

Install Docker Compose:

```
curl -L "https://github.com/docker/compose/releases/download/1.29.1/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
chmod +x /usr/local/bin/docker-compose
```

Install unzip:

```
apt install unzip
```

Install/start/stop/uninstall the application:

```
wget https://www.dropbox.com/s/s4caebj5v9pni37/CloudTest.zip
unzip CloudTest.zip
chmod +x docker-setup.sh
./docker-setup.sh
```

## ESP8266 Board

### Arduino IDE Libraries

* [PubSubClient](https://github.com/knolleary/pubsubclient)
* [ArduinoJson](https://github.com/bblanchon/ArduinoJson)
* [LinkedList](https://github.com/ivanseidel/LinkedList)
* [HX711](https://github.com/bogde/HX711)
