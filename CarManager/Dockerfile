# Use the official ASP.NET runtime image as the base for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["CarManager/CarManager.csproj", "CarManager/"]
RUN dotnet restore "CarManager/CarManager.csproj"

# Copy the entire source code and build the project
COPY . .
WORKDIR "/src/CarManager"
RUN dotnet build "CarManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application (create the final optimized output)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CarManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use the base image to run the published app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarManager.dll"]
