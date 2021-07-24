select result in install start stop uninstall
do
	case $result in
		install) docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
		break
		;;
		start) docker-compose start
		break
		;;
		stop) docker-compose stop
		break
		;;
		uninstall) docker-compose down
		break
		;;
	esac
done
