FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY Swapzy.sln .
COPY src/Swapzy.Core/Swapzy.Core.csproj src/Swapzy.Core/
COPY src/Swapzy.Application/Swapzy.Application.csproj src/Swapzy.Application/
COPY src/Swapzy.Infrastructure/Swapzy.Infrastructure.csproj src/Swapzy.Infrastructure/
COPY src/Swapzy.Api/Swapzy.Api.csproj src/Swapzy.Api/
RUN dotnet restore src/Swapzy.Api/Swapzy.Api.csproj

COPY src/ src/
RUN dotnet publish src/Swapzy.Api/Swapzy.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Swapzy.Api.dll"]
