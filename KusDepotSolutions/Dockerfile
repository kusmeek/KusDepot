FROM node:latest AS node_build

WORKDIR /src/KusDepotSolutions
COPY ["KusDepotSolutions/package.json", "KusDepotSolutions/package-lock.json", "./"]
RUN npm install
COPY ["KusDepotSolutions/", "./"]
RUN npm run production

FROM mcr.microsoft.com/dotnet/sdk:11.0-resolute AS build
COPY --from=node_build /usr/local/bin/node /usr/local/bin/
COPY --from=node_build /usr/local/include /usr/local/include
COPY --from=node_build /usr/local/lib/node_modules /usr/local/lib/node_modules
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["KusDepotSolutions/KusDepotSolutions.csproj", "KusDepotSolutions/"]
COPY ["KusDepotSolutions/Nuget.config", "KusDepotSolutions/"]
COPY ["KusDepotSolutions/localpkgs/", "KusDepotSolutions/localpkgs/"]
RUN dotnet restore "KusDepotSolutions/KusDepotSolutions.csproj" --configfile /src/KusDepotSolutions/Nuget.config

COPY ["KusDepotSolutions/", "KusDepotSolutions/"]
COPY --from=node_build /src/KusDepotSolutions/node_modules /src/KusDepotSolutions/node_modules
RUN dotnet publish "KusDepotSolutions/KusDepotSolutions.csproj" -c Release -o /out /p:UseAppHost=false /p:ContainerBuild=true --configfile /src/KusDepotSolutions/Nuget.config
RUN rm -rf /out/wwwroot/*
COPY --from=node_build /src/KusDepotSolutions/wwwroot/ /out/wwwroot/

FROM mcr.microsoft.com/dotnet/aspnet:11.0-resolute

EXPOSE 8080
WORKDIR /app
COPY --from=build /out/ ./

ENTRYPOINT ["dotnet", "KusDepotSolutions.dll"]
