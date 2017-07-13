docker-compose stop
docker rm $(docker ps -a | grep mysql | awk '{ print $1 }')
docker-compose up -d