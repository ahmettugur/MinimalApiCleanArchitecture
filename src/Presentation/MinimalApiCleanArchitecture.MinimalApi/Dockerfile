#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Presentation/MinimalApiCleanArchitecture.MinimalApi/MinimalApiCleanArchitecture.MinimalApi.csproj", "src/Presentation/MinimalApiCleanArchitecture.MinimalApi/"]
COPY ["src/BuildingBlocks/Common.Logging/Common.Logging.csproj", "src/BuildingBlocks/Common.Logging/"]
COPY ["src/BuildingBlocks/Common.OpenTelemetry/Common.OpenTelemetry.csproj", "src/BuildingBlocks/Common.OpenTelemetry/"]
COPY ["src/Infrastructure/MinimalApiCleanArchitecture.Infrastructure/MinimalApiCleanArchitecture.Infrastructure.csproj", "src/Infrastructure/MinimalApiCleanArchitecture.Infrastructure/"]
COPY ["src/Core/MinimalApiCleanArchitecture.Application/MinimalApiCleanArchitecture.Application.csproj", "src/Core/MinimalApiCleanArchitecture.Application/"]
COPY ["src/Core/MinimalApiCleanArchitecture.Domain/MinimalApiCleanArchitecture.Domain.csproj", "src/Core/MinimalApiCleanArchitecture.Domain/"]
COPY ["src/Infrastructure/MinimalApiCleanArchitecture.Persistence/MinimalApiCleanArchitecture.Persistence.csproj", "src/Infrastructure/MinimalApiCleanArchitecture.Persistence/"]
RUN dotnet restore "./src/Presentation/MinimalApiCleanArchitecture.MinimalApi/./MinimalApiCleanArchitecture.MinimalApi.csproj"
COPY . .
WORKDIR "/src/src/Presentation/MinimalApiCleanArchitecture.MinimalApi"
RUN dotnet build "./MinimalApiCleanArchitecture.MinimalApi.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MinimalApiCleanArchitecture.MinimalApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApiCleanArchitecture.MinimalApi.dll"]