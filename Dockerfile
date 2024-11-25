# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS base
WORKDIR /app

EXPOSE 4000
ENV ASPNETCORE_URLS=http://+:4000

FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS build
WORKDIR /tmp
COPY ["src/MtgBans.Api/MtgBans.Api.csproj", "src/MtgBans.Api/"]
RUN dotnet restore "src/MtgBans.Api/MtgBans.Api.csproj"
COPY . .
WORKDIR "/tmp/src/MtgBans.Api"
RUN dotnet build "MtgBans.Api.csproj" -c Release

FROM build AS publish
RUN dotnet publish "MtgBans.Api.csproj" --no-build -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MtgBans.Api.dll"]
