FROM mysql:latest
ADD IdentityTablesMySql.sql /docker-entrypoint-initdb.d/
ENV MYSQL_ROOT_PASSWORD=123456
ENV MYSQL_DATABASE=identity
ENV MYSQL_USER=identity
ENV MYSQL_PASSWORD=123456
EXPOSE 3306