# Cloud Test

## Run on Ubuntu

Update Ubuntu:

```
apt update
apt upgrade
```

Install Docker:

```
apt install docker.io
```

Install MQTT broker:

```
docker pull eclipse-mosquitto
docker run -it -p 1883:1883 -d --restart always --name mosquitto eclipse-mosquitto mosquitto -c /mosquitto-no-auth.conf
```

## Configuring the ESP Board

The board initially creates a temporary wireless network with the name `CloudTest_xxxxx`. Connect to this network and open the Wi-Fi Manager by navigating to
web page `192.168.4.1`.
