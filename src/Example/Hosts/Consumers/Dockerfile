ARG runtime="mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim"
ARG sdk="mcr.microsoft.com/dotnet/sdk:8.0"

FROM ${sdk} AS build

WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Build.targets", "./"]

COPY ["Common/Contracts/.", "Common/Contracts/"]
COPY ["Common/Clients/.", "Common/Clients/"]
COPY ["Common/Hosts/Common/.", "Common/Hosts/Common/"]
COPY ["Common/Hosts/Consumer/.", "Common/Hosts/Consumer/"]

COPY ["Example/Contracts/.", "Example/Contracts/"]
COPY ["Example/Clients/.", "Example/Clients/"]
COPY ["Example/Hosts/Consumers/.", "Example/Hosts/Consumers/"]

RUN dotnet restore "./Example/Hosts/Consumers/Meta.Example.Hosts.Consumer.csproj"

WORKDIR "/src/Example/Hosts/Consumers"
RUN dotnet build "./Meta.Example.Hosts.Consumer.csproj" -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "./Meta.Example.Hosts.Consumer.csproj" -o /app/publish /p:UseAppHost=false

FROM ${runtime} AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meta.Example.Hosts.Consumer.dll"]