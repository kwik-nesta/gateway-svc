# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/KwikNesta.Gateway.Svc.API/KwikNesta.Gateway.Svc.API.csproj", "src/KwikNesta.Gateway.Svc.API/"]
COPY ["src/KwikNesta.Gateway.Svc.Application/KwikNesta.Gateway.Svc.Application.csproj", "src/KwikNesta.Gateway.Svc.Application/"]
COPY ["src/KwikNesta.Gateway.Svc.Infrastructure/KwikNesta.Gateway.Svc.Infrastructure.csproj", "src/KwikNesta.Gateway.Svc.Infrastructure/"]
RUN dotnet restore "./src/KwikNesta.Gateway.Svc.API/KwikNesta.Gateway.Svc.API.csproj"
COPY . .
WORKDIR "/src/src/KwikNesta.Gateway.Svc.API"
RUN dotnet build "./KwikNesta.Gateway.Svc.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KwikNesta.Gateway.Svc.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KwikNesta.Gateway.Svc.API.dll"]