# DataPod Database Utility

## Commands

- `dotnet run --project KusDepotDataPod/Database/KusDepotDataPod.Database.csproj -- --list-catalogs`
- `dotnet run --project KusDepotDataPod/Database/KusDepotDataPod.Database.csproj -- --migrate-catalogs [--catalog <name>]`
- `dotnet run --project KusDepotDataPod/Database/KusDepotDataPod.Database.csproj -- --migrate-datacontrol`
- `dotnet run --project KusDepotDataPod/Database/KusDepotDataPod.Database.csproj -- --migrate-all`

`--migrate-all` migrates all active catalogs and DataControl. Migration commands accept `--config <file-path>`; `--catalog <name>` applies to `--list-catalogs` and `--migrate-catalogs`. Paths are relative to the current working directory.

## Configuration

Precedence per key, highest first: environment variables, JSON file, Azure App Configuration. See [database.example.json](database.example.json).

- DataControl uses `DataControl:Database:*` in Azure App Configuration and JSON, and `DataControl__Database__*` environment variables.
- Catalog host and port use `CatalogDB:Database:Address` and `CatalogDB:Database:Port` in JSON, or `CatalogDB__Database__Address` and `CatalogDB__Database__Port` as environment variables.
- A nonempty `DataControl:Database:ConnectionString` supplies the host and port; explicit address or port values must match, even across providers. If both specify a database name, `Name` must match the connection string. To use separate settings, override an inherited connection string with an empty value.
- PostgreSQL credentials come from Key Vault or Microsoft Entra, not configuration or connection strings.

Example: `dotnet run --project KusDepotDataPod/Database/KusDepotDataPod.Database.csproj -- --migrate-all --config .\database.local.json`
