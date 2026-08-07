FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["DataServices/DataServices.csproj", "DataServices/"]
COPY ["KusDepot.BLDRS/KusDepot.BLDRS.csproj", "KusDepot.BLDRS/"]
COPY ["KusDepot.Objects/KusDepot.Objects.csproj", "KusDepot.Objects/"]
COPY ["KusDepot.SourceGen/KusDepot.SourceGen.csproj", "KusDepot.SourceGen/"]
COPY ["KusDepotDataPod/Catalog/KusDepotDataPod.Catalog.csproj", "KusDepotDataPod/Catalog/"]
COPY ["KusDepotDataPod/DataControl/KusDepotDataPod.DataControl.csproj", "KusDepotDataPod/DataControl/"]
COPY ["KusDepotDataPod/Services/KusDepotDataPod.Services.csproj", "KusDepotDataPod/Services/"]
COPY ["KusDepotDataPod/Shared/KusDepotDataPod.Shared.csproj", "KusDepotDataPod/Shared/"]

RUN dotnet restore "KusDepotDataPod/Catalog/KusDepotDataPod.Catalog.csproj"
RUN dotnet restore "KusDepotDataPod/DataControl/KusDepotDataPod.DataControl.csproj"
RUN dotnet restore "KusDepotDataPod/Services/KusDepotDataPod.Services.csproj"

COPY ["DataServices/", "DataServices/"]
COPY ["KusDepot.BLDRS/", "KusDepot.BLDRS/"]
COPY ["KusDepot.Objects/", "KusDepot.Objects/"]
COPY ["KusDepot.SourceGen/", "KusDepot.SourceGen/"]
COPY ["KusDepot.Packages/images/", "KusDepot.Packages/images/"]
COPY ["KusDepotDataPod/Catalog/", "KusDepotDataPod/Catalog/"]
COPY ["KusDepotDataPod/DataControl/", "KusDepotDataPod/DataControl/"]
COPY ["KusDepotDataPod/Services/", "KusDepotDataPod/Services/"]
COPY ["KusDepotDataPod/Shared/", "KusDepotDataPod/Shared/"]

RUN dotnet publish "KusDepotDataPod/Catalog/KusDepotDataPod.Catalog.csproj" -c Release -o /app/catalog /p:UseAppHost=false /p:ContainerBuild=true
RUN dotnet publish "KusDepotDataPod/DataControl/KusDepotDataPod.DataControl.csproj" -c Release -o /app/datacontrol /p:UseAppHost=false /p:ContainerBuild=true
RUN dotnet publish "KusDepotDataPod/Services/KusDepotDataPod.Services.csproj" -c Release -o /app/services /p:UseAppHost=false /p:ContainerBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble
WORKDIR /app

ENV KUSDEPOT_DATAPOD_HOME=/app/DataPodHome

COPY --from=build /app/catalog ./catalog
COPY --from=build /app/datacontrol ./datacontrol
COPY --from=build /app/services ./services
COPY KusDepotDataPod/App/DataPodHome ./DataPodHome
COPY KusDepotDataPod/App/DataPodHome.Container ./DataPodHome
COPY KusDepotDataPod/App/entry.sh ./entry.sh

RUN mkdir -p /app/DataPodHome/data \
    /app/DataPodHome/logs/Catalog \
    /app/DataPodHome/logs/CatalogDB \
    /app/DataPodHome/logs/DataConfiguration \
    /app/DataPodHome/logs/DataControl \
    /app/DataPodHome/logs/DataPodSilo \
    && cp /app/DataPodHome/certificates/root/root.cer /usr/local/share/ca-certificates/kusdepot-root.crt \
    && sed -i 's/\r$//' /app/entry.sh \
    && update-ca-certificates \
    && chmod +x /app/entry.sh

EXPOSE 5006 5007 11115 30003

ENTRYPOINT ["bash", "/app/entry.sh"]
