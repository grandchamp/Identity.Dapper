docker stop $(docker ps -a | grep sqlserver | awk '{ print $1 }')
docker rm $(docker ps -a | grep sqlserver | awk '{ print $1 }')
docker run -it -p 1433:1433 -d sqlserver_integration