#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["microservice-map-tester/microservice-map-tester.csproj", "microservice-map-tester/"]
RUN dotnet restore "microservice-map-tester/microservice-map-tester.csproj"
COPY . .
WORKDIR "/src/microservice-map-tester"
RUN dotnet build "microservice-map-tester.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "microservice-map-tester.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "microservice-map-tester.dll"]