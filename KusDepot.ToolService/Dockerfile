FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["NuGet.config", "."]
COPY ["KusDepot.ToolService/KusDepot.ToolService.csproj", "KusDepot.ToolService/"]
COPY ["KusDepot.Objects/KusDepot.Objects.csproj", "KusDepot.Objects/"]
RUN dotnet restore "KusDepot.ToolService/KusDepot.ToolService.csproj"
COPY . .
WORKDIR "/src/KusDepot.ToolService"
RUN dotnet build "KusDepot.ToolService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KusDepot.ToolService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToolService.dll"]