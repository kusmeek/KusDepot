FROM mcr.microsoft.com/dotnet/sdk:11.0-resolute AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["DataServices/DataServices.csproj", "DataServices/"]
COPY ["KusDepot.BLDRS/KusDepot.BLDRS.csproj", "KusDepot.BLDRS/"]
COPY ["KusDepot.Objects/KusDepot.Objects.csproj", "KusDepot.Objects/"]
COPY ["KusDepot.SourceGen/KusDepot.SourceGen.csproj", "KusDepot.SourceGen/"]
COPY ["KusDepotDataPod/Services/KusDepotDataPod.Services.csproj", "KusDepotDataPod/Services/"]
COPY ["KusDepotDataPod/Shared/KusDepotDataPod.Shared.csproj", "KusDepotDataPod/Shared/"]

RUN dotnet restore "KusDepotDataPod/Services/KusDepotDataPod.Services.csproj"

COPY ["DataServices/", "DataServices/"]
COPY ["KusDepot.BLDRS/", "KusDepot.BLDRS/"]
COPY ["KusDepot.Objects/", "KusDepot.Objects/"]
COPY ["KusDepot.SourceGen/", "KusDepot.SourceGen/"]
COPY ["KusDepot.Packages/images/", "KusDepot.Packages/images/"]
COPY ["KusDepotDataPod/Services/", "KusDepotDataPod/Services/"]
COPY ["KusDepotDataPod/Shared/", "KusDepotDataPod/Shared/"]

RUN dotnet publish "KusDepotDataPod/Services/KusDepotDataPod.Services.csproj" -c Release -o /out /p:UseAppHost=false /p:ContainerBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:11.0-resolute
WORKDIR /app

ENV KUSDEPOT_DATAPOD_HOME=/app/DataPodHome

RUN apt-get update \
	&& apt-get install -y --no-install-recommends libgssapi-krb5-2 \
	&& rm -rf /var/lib/apt/lists/* \
	&& mkdir -p /app/DataPodHome/logs/DataPodSilo \
		/app/DataPodHome/logs/CatalogDB \
		/app/DataPodHome/logs/DataConfiguration

COPY --from=build /out/ ./

EXPOSE 11115 30003

ENTRYPOINT ["dotnet", "KusDepotDataPod.Services.dll"]
