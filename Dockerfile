FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY src/*.csproj ./src/
COPY tests/*.csproj ./tests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Run tests
RUN dotnet test --verbosity detailed
# Build and publish if tests pass
RUN dotnet publish src -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000

ENTRYPOINT ["dotnet", "FootballManager.dll"]
