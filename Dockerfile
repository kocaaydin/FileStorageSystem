FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src

COPY ["src/FileStorageSystem.Core/FileStorageSystem.Core.csproj", "src/FileStorageSystem.Core/"]
COPY ["src/FileStorageSystem.Data/FileStorageSystem.Data.csproj", "src/FileStorageSystem.Data/"]
COPY ["src/FileStorageSystem.Services/FileStorageSystem.Services.csproj", "src/FileStorageSystem.Services/"]
COPY ["src/FileStorageSystem.StorageProviders/FileStorageSystem.StorageProviders.csproj", "src/FileStorageSystem.StorageProviders/"]
COPY ["src/FileStorageSystem.ConsoleApp/FileStorageSystem.ConsoleApp.csproj", "src/FileStorageSystem.ConsoleApp/"]

RUN dotnet restore "src/FileStorageSystem.ConsoleApp/FileStorageSystem.ConsoleApp.csproj"
COPY . .
RUN dotnet publish "src/FileStorageSystem.ConsoleApp/FileStorageSystem.ConsoleApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5005
ENTRYPOINT ["dotnet", "FileStorageSystem.ConsoleApp.dll"]