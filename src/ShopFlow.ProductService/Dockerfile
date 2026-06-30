FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ShopFlow.ProductService/ShopFlow.ProductService.csproj", "src/ShopFlow.ProductService/"]
COPY ["src/ShopFlow.Contracts/ShopFlow.Contracts.csproj", "src/ShopFlow.Contracts/"]
COPY ["src/ShopFlow.Common/ShopFlow.Common.csproj", "src/ShopFlow.Common/"]
RUN dotnet restore "src/ShopFlow.ProductService/ShopFlow.ProductService.csproj"
COPY . .
WORKDIR "/src/src/ShopFlow.ProductService"
RUN dotnet build "./ShopFlow.ProductService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShopFlow.ProductService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopFlow.ProductService.dll"]
