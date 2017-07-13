docker stop $(docker ps -a | grep psql | awk '{ print $1 }')
docker rm $(docker ps -a | grep psql | awk '{ print $1 }')
docker run -it -p 5432:5432 -d psql_integration