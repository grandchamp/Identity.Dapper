This integration test uses a DOCKERFILE for SQL Server.
Run the DOCKERFILE with your local Docker before starting tests.

If it's the first time running the Docker container, cd to the folder containing Dockerfile and run:
docker build -t sqlserver_integration .

After all the build process, run:

docker run -it -p 1433:1433 -d sqlserver_integration

Check your docker host IP and change the connection string properly.