# Patients tesk task

## Run app & mssql db in docker

```bash
# From repo dir 
docker compose up --build
# wait a bit until sql server is up and running
```

Default forwarded app port - **8000**

Default sql server forwarded port - **1400**

### Postman scripts: /Postman

### ApiRoot: http://localhost:8000/api/v1/patients
### Swagger: http://localhost:8000/swagger/index.html

## Run populate db script
```shell
# From repo dir 
cd PopulateDB
dotnet run 
```

Patients birthdate generation ranges:
```csharp
BirthDate = DateTime.Parse("1900-01-01")
    .AddYears((int)rand.NextInt64(0, 100))
    .AddDays(rand.NextInt64(0, 25))
    .AddMonths((int)rand.NextInt64(0, 10))
    .AddHours((int)rand.NextInt64(0, 22))
    .AddMinutes((int)rand.NextInt64(0, 58))
    .AddSeconds((int)rand.NextInt64(0, 58))
    .ToString()
```
