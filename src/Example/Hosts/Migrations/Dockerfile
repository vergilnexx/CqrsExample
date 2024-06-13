ARG runtime="mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim"
ARG sdk="mcr.microsoft.com/dotnet/sdk:8.0"

FROM ${sdk} AS build

WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Build.targets", "./"]

COPY ["Common/Hosts/Common/.", "Common/Hosts/Common/"]
COPY ["Common/Contracts/.", "Common/Contracts/"]
COPY ["Common/Domain/.", "Common/Domain/"]
COPY ["Common/Infrastructure/.", "Common/Infrastructure/"]

COPY ["Example/Domain/.", "Example/Domain/"]
COPY ["Example/Infrastructure/DataAccess/.", "Example/Infrastructure/DataAccess/"]
COPY ["Example/Hosts/Migrations/.", "Example/Hosts/Migrations/"]

RUN dotnet restore "./Example/Hosts/Migrations/Meta.Example.Hosts.Migrations.csproj"

WORKDIR "/src/Example/Hosts/Migrations"
RUN dotnet build "./Meta.Example.Hosts.Migrations.csproj" -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "./Meta.Example.Hosts.Migrations.csproj" -o /app/publish /p:UseAppHost=false

FROM ${runtime} AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meta.Example.Hosts.Migrations.dll"]