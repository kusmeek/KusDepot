FROM node:latest AS node_base

FROM node:latest AS node_build

WORKDIR /src
COPY package.json package-lock.json .
RUN npm install
COPY . .
RUN npm run development

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-noble AS build

COPY --from=node_base /usr/local/bin/node /usr/local/bin/
COPY --from=node_base /usr/local/include /usr/local/include
COPY --from=node_base /usr/local/lib/node_modules /usr/local/lib/node_modules

WORKDIR /src
COPY . .
COPY --from=node_build /src/node_modules ./node_modules

RUN dotnet publish "Reacts.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:ContainerBuild=true --configfile /src/Nuget.config 
RUN rm -rf /app/publish/wwwroot/*
COPY --from=node_build /src/wwwroot /app/publish/wwwroot

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-noble-chiseled-extra AS final

EXPOSE 8080
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KusDepot.Reacts.dll"]