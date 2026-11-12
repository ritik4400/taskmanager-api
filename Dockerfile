
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file and restore NuGet packages
COPY taskmanager-api.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Publish the application
RUN dotnet publish taskmanager-api.csproj -c Release -o /app/publish --no-restore


# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Application listens on port 8080 inside the container
ENV PORT=8080

EXPOSE 8080

# Start the API
ENTRYPOINT ["dotnet", "taskmanager-api.dll"]
