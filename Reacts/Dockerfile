FROM node:latest AS node_build

WORKDIR /src/Reacts
COPY ["Reacts/package.json", "Reacts/package-lock.json", "./"]
RUN npm install
COPY ["Reacts/", "./"]
RUN npm run development

FROM mcr.microsoft.com/dotnet/sdk:11.0-resolute AS build
COPY --from=node_build /usr/local/bin/node /usr/local/bin/
COPY --from=node_build /usr/local/include /usr/local/include
COPY --from=node_build /usr/local/lib/node_modules /usr/local/lib/node_modules
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["Reacts/Reacts.csproj", "Reacts/"]
COPY ["Reacts/Nuget.config", "Reacts/"]
COPY ["Reacts/localpkgs/", "Reacts/localpkgs/"]
RUN dotnet restore "Reacts/Reacts.csproj" --configfile /src/Reacts/Nuget.config

COPY ["Reacts/", "Reacts/"]
COPY --from=node_build /src/Reacts/node_modules /src/Reacts/node_modules
RUN dotnet publish "Reacts/Reacts.csproj" -c Release -o /out /p:UseAppHost=false /p:ContainerBuild=true --configfile /src/Reacts/Nuget.config
RUN rm -rf /out/wwwroot/*
COPY --from=node_build /src/Reacts/wwwroot/ /out/wwwroot/

FROM mcr.microsoft.com/dotnet/aspnet:11.0-resolute AS final

EXPOSE 8080
WORKDIR /app
COPY --from=build /out/ ./

ENTRYPOINT ["dotnet", "KusDepot.Reacts.dll"]