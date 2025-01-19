FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Calmska.Api/Calmska.Api.csproj", "Calmska.Api/"]
COPY ["Calmska.Models/Calmska.Models.csproj", "Calmska.Models/"]
RUN dotnet restore "./Calmska.Api/Calmska.Api.csproj"
COPY . .
WORKDIR "/src/Calmska.Api"
RUN dotnet build "./Calmska.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Calmska.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calmska.Api.dll"]