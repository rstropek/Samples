docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=expell!armus" -e "MSSQL_PID=Express" \
    -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest 
