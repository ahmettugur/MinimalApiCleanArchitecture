FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Consumers/MinimalApiCleanArchitecture.LogConsumer/MinimalApiCleanArchitecture.LogConsumer.csproj", "src/Consumers/MinimalApiCleanArchitecture.LogConsumer/"]
RUN dotnet restore "src/Consumers/MinimalApiCleanArchitecture.LogConsumer/MinimalApiCleanArchitecture.LogConsumer.csproj"
COPY . .
WORKDIR "/src/src/Consumers/MinimalApiCleanArchitecture.LogConsumer"
RUN dotnet build "MinimalApiCleanArchitecture.LogConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinimalApiCleanArchitecture.LogConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApiCleanArchitecture.LogConsumer.dll"]
