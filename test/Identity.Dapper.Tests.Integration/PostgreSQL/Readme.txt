This integration test uses a TMPFS DOCKERFILE for PSQL.
Run the DOCKERFILE with your local Docker before starting tests.

If it's the first time running the Docker container, cd to the folder containing Dockerfile and run:
docker build -t psql_integration .

After all the build process, run:

docker run -it -p 5432:5432 -d psql_integration

Check your docker host IP and change the connection string properly.