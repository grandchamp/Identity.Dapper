This integration test uses a TMPFS DOCKERFILE for MySQL.
Run the DOCKERFILE with your local Docker before starting tests.

If it's the first time running the Docker container, cd to the folder containing Dockerfile and run:
docker build -t mysql_identity .

After all the build process, run:

docker-compose up -d

Check your docker host IP and change the connection string properly.