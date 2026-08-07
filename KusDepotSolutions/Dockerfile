FROM node:latest AS node_build

WORKDIR /src/KusDepotSolutions
COPY ["KusDepotSolutions/package.json", "KusDepotSolutions/package-lock.json", "./"]
RUN npm install
COPY ["KusDepotSolutions/", "./"]
RUN npm run production

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build

COPY --from=node_build /usr/local/bin/node /usr/local/bin/
COPY --from=node_build /usr/local/include /usr/local/include
COPY --from=node_build /usr/local/lib/node_modules /usr/local/lib/node_modules

WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["KusDepotSolutions/KusDepotSolutions.csproj", "KusDepotSolutions/"]
COPY ["KusDepotSolutions/Nuget.config", "KusDepotSolutions/"]
COPY ["KusDepotSolutions/localpkgs/", "KusDepotSolutions/localpkgs/"]
RUN dotnet restore "KusDepotSolutions/KusDepotSolutions.csproj" --configfile /src/KusDepotSolutions/Nuget.config
COPY ["KusDepotSolutions/", "KusDepotSolutions/"]
COPY --from=node_build /src/KusDepotSolutions/node_modules /src/KusDepotSolutions/node_modules

RUN dotnet publish "KusDepotSolutions/KusDepotSolutions.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:ContainerBuild=true --configfile /src/KusDepotSolutions/Nuget.config
RUN rm -rf /app/publish/wwwroot/*
COPY --from=node_build /src/KusDepotSolutions/wwwroot /app/publish/wwwroot

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS final

EXPOSE 8080
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KusDepotSolutions.dll"]
