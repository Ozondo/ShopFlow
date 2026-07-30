FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ShopFlow.OrderService/ShopFlow.OrderService.csproj", "src/ShopFlow.OrderService/"]
COPY ["src/ShopFlow.Contracts/ShopFlow.Contracts.csproj", "src/ShopFlow.Contracts/"]
COPY ["src/ShopFlow.Common/ShopFlow.Common.csproj", "src/ShopFlow.Common/"]
RUN dotnet restore "src/ShopFlow.OrderService/ShopFlow.OrderService.csproj"
COPY . .
WORKDIR "/src/src/ShopFlow.OrderService"
RUN dotnet build "./ShopFlow.OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShopFlow.OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopFlow.OrderService.dll"]
