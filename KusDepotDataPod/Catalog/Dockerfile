FROM mcr.microsoft.com/dotnet/sdk:11.0-resolute AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["DataServices/DataServices.csproj", "DataServices/"]
COPY ["KusDepot.BLDRS/KusDepot.BLDRS.csproj", "KusDepot.BLDRS/"]
COPY ["KusDepot.Objects/KusDepot.Objects.csproj", "KusDepot.Objects/"]
COPY ["KusDepot.SourceGen/KusDepot.SourceGen.csproj", "KusDepot.SourceGen/"]
COPY ["KusDepotDataPod/Catalog/KusDepotDataPod.Catalog.csproj", "KusDepotDataPod/Catalog/"]
COPY ["KusDepotDataPod/Shared/KusDepotDataPod.Shared.csproj", "KusDepotDataPod/Shared/"]

RUN dotnet restore "KusDepotDataPod/Catalog/KusDepotDataPod.Catalog.csproj"

COPY ["DataServices/", "DataServices/"]
COPY ["KusDepot.BLDRS/", "KusDepot.BLDRS/"]
COPY ["KusDepot.Objects/", "KusDepot.Objects/"]
COPY ["KusDepot.SourceGen/", "KusDepot.SourceGen/"]
COPY ["KusDepot.Packages/images/", "KusDepot.Packages/images/"]
COPY ["KusDepotDataPod/Catalog/", "KusDepotDataPod/Catalog/"]
COPY ["KusDepotDataPod/Shared/", "KusDepotDataPod/Shared/"]

RUN dotnet publish "KusDepotDataPod/Catalog/KusDepotDataPod.Catalog.csproj" -c Release -o /out /p:UseAppHost=false /p:ContainerBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:11.0-resolute
WORKDIR /app

ENV KUSDEPOT_DATAPOD_HOME=/app/DataPodHome

RUN mkdir -p /app/DataPodHome/logs/Catalog

COPY --from=build /out/ ./

EXPOSE 5006

ENTRYPOINT ["dotnet", "KusDepotDataPod.Catalog.dll"]
