# taskmanager-api
A Task Manager REST API built using ASP.NET Core Web API, Entity Framework Core and PostgreSQL.

# steps

1. Clone the Repo.
2. Verify your branch -> git branch
3. check dotnet version -> dotnet --version
4. create the Web API project in the current directory
   -> dotnet new webapi --use-controllers 

# Run your application

1. dotnet restore
2. dotnet build 
3. dotnet run 

# Push your code project to GitHub.

- git status.
- git add .
- git commit -m "Project updated"
- git push .

# dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=YOUR_POOLER_HOST;Port=5432;Database=postgres;Username=postgres.YOUR_PROJECT_REF;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"

# Cheak app health status 
http://localhost:5162/api/health

# Cheak db connection status
http://localhost:5162/api/health/db

# Migrations - First Migration
dotnet ef migrations add InitialCreate

dotnet ef database update

