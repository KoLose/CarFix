# Build Avalonia app for Linux, serve UI through noVNC
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Domain/Domain.csproj Domain/
COPY Infastructure/Infastructure.csproj Infastructure/
COPY AvaloniaApp/AvaloniaApp.csproj AvaloniaApp/
RUN dotnet restore AvaloniaApp/AvaloniaApp.csproj

COPY Domain/ Domain/
COPY Infastructure/ Infastructure/
COPY AvaloniaApp/ AvaloniaApp/
RUN dotnet publish AvaloniaApp/AvaloniaApp.csproj -c Release -r linux-x64 --self-contained false -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    libice6 libsm6 libfontconfig1 libx11-6 libxrandr2 libxi6 \
    libgl1 libglu1-mesa \
    xvfb x11vnc novnc websockify \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
COPY AvaloniaApp/appsettings.json ./appsettings.json
COPY docker/start-app.sh /start-app.sh
RUN chmod +x /start-app.sh

ENV ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=carfixdb;Username=postgres;Password=123"
EXPOSE 6080

ENTRYPOINT ["/start-app.sh"]
