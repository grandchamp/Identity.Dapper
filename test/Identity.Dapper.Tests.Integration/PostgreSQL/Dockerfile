FROM postgres:latest
ENV POSTGRES_USER identity
ENV POSTGRES_PASSWORD 123456
ENV POSTGRES_DB identity
ENV PGDATA /dev/shm/pgdata/data
ADD IdentityTablesPostgreSql.sql /docker-entrypoint-initdb.d/
EXPOSE 5432