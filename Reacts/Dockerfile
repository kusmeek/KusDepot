FROM node:latest AS node_base

FROM node:latest AS node_build

WORKDIR /src/Reacts
COPY ["Reacts/package.json", "Reacts/package-lock.json", "./"]
RUN npm install
COPY ["Reacts/", "./"]
RUN npm run development

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build

COPY --from=node_base /usr/local/bin/node /usr/local/bin/
COPY --from=node_base /usr/local/include /usr/local/include
COPY --from=node_base /usr/local/lib/node_modules /usr/local/lib/node_modules

WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Reacts/Reacts.csproj", "Reacts/"]
COPY ["Reacts/Nuget.config", "Reacts/"]
COPY ["Reacts/localpkgs/", "Reacts/localpkgs/"]
RUN dotnet restore "Reacts/Reacts.csproj" --configfile /src/Reacts/Nuget.config
COPY ["Reacts/", "Reacts/"]
COPY --from=node_build /src/Reacts/node_modules /src/Reacts/node_modules

RUN dotnet publish "Reacts/Reacts.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:ContainerBuild=true --configfile /src/Reacts/Nuget.config
RUN rm -rf /app/publish/wwwroot/*
COPY --from=node_build /src/Reacts/wwwroot /app/publish/wwwroot

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS final

EXPOSE 8080
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KusDepot.Reacts.dll"]