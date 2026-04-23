# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ExamCenterSystem.csproj .
RUN dotnet restore

# Copy all files and build
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app .

# Run the application
ENTRYPOINT ["dotnet", "ExamCenterSystem.dll"]