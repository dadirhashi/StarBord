# ---------- Steg 1: build ----------
# --platform=$BUILDPLATFORM: kör SDK:n på DIN processor (snabbt),
# men kompilera för målets processor via -a $TARGETARCH
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY StarBord.csproj ./
RUN dotnet restore StarBord.csproj -a $TARGETARCH

COPY . .
RUN dotnet publish StarBord.csproj -c Release -o /app/publish --no-restore -a $TARGETARCH /p:UseAppHost=false

# ---------- Steg 2: runtime ----------
# aspnet-imagen kan bara KÖRA appar, inte bygga: mindre och färre säkerhetshål
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Ta bara med den färdiga appen från steg 1, inte källkod eller SDK
COPY --from=build /app/publish .

# Lyssna på port 8080. Portar under 1024 kräver root, 8080 gör det inte.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Kör INTE som root. Microsofts image har en färdig användare "app".
USER $APP_UID

ENTRYPOINT ["dotnet", "StarBord.dll"]