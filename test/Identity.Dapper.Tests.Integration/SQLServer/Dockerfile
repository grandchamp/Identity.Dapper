FROM genschsa/mssql-server-linux
ENV ACCEPT_EULA Y
ENV SA_PASSWORD test@123!
ADD IdentityTablesSqlServer.sql /docker-entrypoint-initdb.d/
EXPOSE 1433