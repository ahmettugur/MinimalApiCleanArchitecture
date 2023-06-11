FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Presentation/MinimalApiCleanArchitecture.GrpcService/MinimalApiCleanArchitecture.GrpcService.csproj", "src/Presentation/MinimalApiCleanArchitecture.GrpcService/"]
COPY ["src/BuildingBlocks/Common.Logging/Common.Logging.csproj", "src/BuildingBlocks/Common.Logging/"]
COPY ["src/BuildingBlocks/Common.OpenTelemetry/Common.OpenTelemetry.csproj", "src/BuildingBlocks/Common.OpenTelemetry/"]
COPY ["src/Infrastructure/MinimalApiCleanArchitecture.Persistence/MinimalApiCleanArchitecture.Persistence.csproj", "src/Infrastructure/MinimalApiCleanArchitecture.Persistence/"]
COPY ["src/Core/MinimalApiCleanArchitecture.Application/MinimalApiCleanArchitecture.Application.csproj", "src/Core/MinimalApiCleanArchitecture.Application/"]
COPY ["src/Core/MinimalApiCleanArchitecture.Domain/MinimalApiCleanArchitecture.Domain.csproj", "src/Core/MinimalApiCleanArchitecture.Domain/"]
RUN dotnet restore "src/Presentation/MinimalApiCleanArchitecture.GrpcService/MinimalApiCleanArchitecture.GrpcService.csproj"
COPY . .
WORKDIR "/src/src/Presentation/MinimalApiCleanArchitecture.GrpcService"
RUN dotnet build "MinimalApiCleanArchitecture.GrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinimalApiCleanArchitecture.GrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApiCleanArchitecture.GrpcService.dll"]
