FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-noble AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReactF/ReactF.fsproj", "ReactF/"]
COPY ["ReactG/ReactG.csproj", "ReactG/"]
RUN dotnet restore "./ReactF/ReactF.fsproj"
COPY ["ReactF/", "ReactF/"]
COPY ["ReactG/", "ReactG/"]
WORKDIR "/src/ReactF"
RUN dotnet publish "./ReactF.fsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:ContainerBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-noble-chiseled-extra
WORKDIR /app
EXPOSE 8081 8082
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KusDepot.ReactF.dll"]