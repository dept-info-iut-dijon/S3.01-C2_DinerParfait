
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081

# 
iables d'environnement pour les ports et l'environnement
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8081
ENV ASPNETCORE_URLS="http://*:8081"

RUN chown app:app /app
USER app

# Image pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les fichiers .csproj avec l'arborescence d'origine
COPY ["EpicurApp-API/EpicurApp-API/EpicurApp-API.csproj", "EpicurApp-API/EpicurApp-API/"]
COPY ["EpicurApp-API/EpicurAppLogic/EpicurAppLogic.csproj", "EpicurApp-API/EpicurAppLogic/"]
# Copier le projet de partage
COPY ["EpicurApp/EpicurAPP-Partage/EpicurAPP-Partage/EpicurAPP-Partage.csproj", "EpicurApp/EpicurAPP-Partage/EpicurAPP-Partage/"]

# Restaurer les dépendances depuis l'API
RUN dotnet restore "EpicurApp-API/EpicurApp-API/EpicurApp-API.csproj"

# Copier tout le code source en conservant l'arborescence
COPY ["EpicurApp-API/EpicurApp-API/", "EpicurApp-API/EpicurApp-API/"]
COPY ["EpicurApp-API/EpicurAppLogic/", "EpicurApp-API/EpicurAppLogic/"]
COPY ["EpicurApp/EpicurAPP-Partage/EpicurAPP-Partage/", "EpicurApp/EpicurAPP-Partage/EpicurAPP-Partage/"]

# Compiler le projet
WORKDIR "/src/EpicurApp-API/EpicurApp-API"
RUN dotnet build "EpicurApp-API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publier l'application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/EpicurApp-API/EpicurApp-API"
RUN dotnet publish "EpicurApp-API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish --chown=app:app /app/publish .

RUN chmod 664 /app/epicurapp.db || true

RUN mkdir -p /app/data && chmod 777 /app/data


ENTRYPOINT ["dotnet", "EpicurApp-API.dll"]