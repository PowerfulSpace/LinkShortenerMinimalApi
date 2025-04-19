# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем остальной код и билдим
COPY . ./
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Устанавливаем переменную окружения
ENV ASPNETCORE_ENVIRONMENT=Docker

# Копируем билд
COPY --from=build /out ./

# Открываем порт (внутренний)
EXPOSE 8080

# Точка входа
ENTRYPOINT ["dotnet", "PS.LinkShortenerMinimalApi.Web.dll"]
