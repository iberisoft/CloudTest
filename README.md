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
docker run -dit -p 1883:1883 --restart always --name mosquitto eclipse-mosquitto mosquitto -c /mosquitto-no-auth.conf
```
