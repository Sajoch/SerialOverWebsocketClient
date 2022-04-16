# Build CS project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app

COPY SerialOverWebsocketClient/*.csproj ./
RUN dotnet restore

COPY SerialOverWebsocketClient/* ./
RUN dotnet publish -c Release -o out

# Runner
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runner
WORKDIR /app

COPY --from=builder /app/out/*.dll ./
COPY --from=builder /app/out/*.runtimeconfig.json ./
COPY --from=builder /app/out/appsettings.json ./Settings/appsettings.json
ENTRYPOINT ["dotnet", "SerialOverWebsocketClient.dll"]