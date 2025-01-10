#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Consumers/MinimalApiCleanArchitecture.LogConsumer/MinimalApiCleanArchitecture.LogConsumer.csproj", "src/Consumers/MinimalApiCleanArchitecture.LogConsumer/"]
RUN dotnet restore "./src/Consumers/MinimalApiCleanArchitecture.LogConsumer/./MinimalApiCleanArchitecture.LogConsumer.csproj"
COPY . .
WORKDIR "/src/src/Consumers/MinimalApiCleanArchitecture.LogConsumer"
RUN dotnet build "./MinimalApiCleanArchitecture.LogConsumer.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MinimalApiCleanArchitecture.LogConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApiCleanArchitecture.LogConsumer.dll"]