ARG runtime="mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim"
ARG sdk="mcr.microsoft.com/dotnet/sdk:8.0"

FROM ${sdk} AS build

WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Build.targets", "./"]

COPY ["Common/Applications/AppServices/.", "Common/Applications/AppServices/"]
COPY ["Common/Applications/Handlers/.", "Common/Applications/Handlers/"]
COPY ["Common/Cqrs/.", "Common/Cqrs/"]
COPY ["Common/Contracts/.", "Common/Contracts/"]
COPY ["Common/Clients/.", "Common/Clients/"]
COPY ["Common/Hosts/Grpc/.", "Common/Hosts/Grpc/"]
COPY ["Common/Hosts/Common/.", "Common/Hosts/Common/"]
COPY ["Common/Domain/.", "Common/Domain/"]
COPY ["Common/Hosts/Consumer/.", "Common/Hosts/Consumer/"]
COPY ["Common/Infrastructure/.", "Common/Infrastructure/"]

COPY ["Example/Applications/Handlers/.", "Example/Applications/Handlers/"]
COPY ["Example/Applications/AppServices/.", "Example/Applications/AppServices/"]
COPY ["Example/Contracts/.", "Example/Contracts/"]
COPY ["Example/Clients/.", "Example/Clients/"]
COPY ["Example/Domain/.", "Example/Domain/"]
COPY ["Example/Infrastructure/DataAccess/.", "Example/Infrastructure/DataAccess/"]
COPY ["Example/Hosts/Private/Grpc/.", "Example/Hosts/Private/Grpc/"]

RUN dotnet restore "./Example/Hosts/Private/Grpc/Meta.Example.Private.Hosts.Grpc.csproj"

WORKDIR "/src/Example/Hosts/Private/Grpc"
RUN dotnet build "./Meta.Example.Private.Hosts.Grpc.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "./Meta.Example.Private.Hosts.Grpc.csproj" -o /app/publish /p:UseAppHost=false

FROM ${runtime} AS final
EXPOSE 5102
EXPOSE 5103
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meta.Example.Private.Hosts.Grpc.dll"]