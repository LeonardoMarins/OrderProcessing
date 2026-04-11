FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderProcessing.Worker/OrderProcessing.Worker.csproj", "OrderProcessing.Worker/"]
RUN dotnet restore "OrderProcessing.Worker/OrderProcessing.Worker.csproj"
COPY . .
WORKDIR "/src/OrderProcessing.Worker"
RUN dotnet build "./OrderProcessing.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderProcessing.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderProcessing.Worker.dll"]
