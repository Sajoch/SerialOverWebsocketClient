# Build helper lib libpts
FROM ubuntu as builder-pts
RUN apt update && apt upgrade -y

RUN apt install -y gcc make

WORKDIR /app

COPY libpts/* ./
RUN make all

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

COPY --from=builder-pts /app/libpts.so ./
COPY --from=builder /app/out/*.dll ./
COPY --from=builder /app/out/*.runtimeconfig.json ./
COPY --from=builder /app/out/Settings ./Settings
ENTRYPOINT ["dotnet", "SerialOverWebsocketClient.dll"]