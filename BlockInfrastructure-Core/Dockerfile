FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["BlockInfrastructure-Core/BlockInfrastructure-Core.csproj", "BlockInfrastructure-Core/"]
RUN dotnet restore "BlockInfrastructure-Core/BlockInfrastructure-Core.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/BlockInfrastructure-Core"
RUN dotnet publish "BlockInfrastructure-Core.csproj" -c Release -o /app/publish /p:UseAppHost=false --self-contained false --no-restore -a $TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlockInfrastructure-Core.dll"]
