FROM mcr.microsoft.com/mssql/server:2019-latest
USER root
WORKDIR /usr/config
COPY ./setup.sql .
ENTRYPOINT /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i ./setup.sql & /opt/mssql/bin/sqlservr