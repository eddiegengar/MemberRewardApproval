# connect database via docker

- Start the colima

```zsh
colima start
```

- Run the SQL container (first time) m4 laptop

```zsh
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1431:1433 \
  -v ~/sql-data:/var/opt/mssql \
  --name sqlserver2022 \
  -d mcr.microsoft.com/azure-sql-edge:latest
```

- Start the container again

```zsh
docker start sqlserver2022
```

- Stop the container

```zsh
docker stop sqlserver2022
```

- Run the SQL container (first time) intel laptop

```zsh
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssw0rd" -p 1431:1433 --name sqlserver1431 -d mcr.microsoft.com/mssql/server:2022-latest
```

# curl request

```zsh
curl -X POST http://localhost:5172/api/RewardRequests \
-H "Content-Type: application/json" \
-d '{"WynnId":"12345678","RewardType":"籌碼兌換申請審批","RequestedValue":{"Title":"申請兌換⾦額:","Amount":200000}}'
```

# drop database

```sql
USE master;
ALTER DATABASE [RewardsDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [RewardsDb]
```

# ef migration

```zsh
dotnet ef migrations add InitialCreate
dotnet ef database update
```

supervisor1@eddiegengargmail.onmicrosoft.com
Covo035307Admin150150!@
