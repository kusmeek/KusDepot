# DataPod configuration

This document describes the process environment and Azure App Configuration schema used by Catalog, DataControl, and DataPodSilo.

## Environment variables

| Variable | Used by | Purpose |
|---|---|---|
| `KUSDEPOT_APPCONFIGURATION_ENDPOINT` | All runtime services; database tooling | Required HTTPS endpoint for the App Configuration store. |
| `KUSDEPOT_KEYVAULT_URI` | All runtime services; database tooling | Required Key Vault URI used for secrets and Azure credentials. |
| `KUSDEPOT_DEPLOYMENT_PROFILE` | All runtime services | Optional profile selector. Set to `AzureContainer` to load that profile's overrides and enable Azure Container Apps Orleans configuration. Leave unset for local/default behavior. |
| `DOTNET_ENVIRONMENT` | All runtime services | Optional App Configuration environment label; defaults to `Production`. |
| `ASPNETCORE_ENVIRONMENT` | All runtime services | Fallback environment label when `DOTNET_ENVIRONMENT` is unset. |
| `AZURE_CLIENT_ID` | All runtime services | Optional user-assigned managed identity client ID. If unset, the system-assigned identity is used. `EnvironmentCredential` is tried first by the credential chain. |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | All runtime services | Optional Azure Monitor exporter configuration for traces, metrics, and logs. |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | All runtime services | Optional fallback OTLP endpoint for traces, metrics, and logs. |
| `OTEL_EXPORTER_OTLP_TRACES_ENDPOINT` | All runtime services | Optional trace-specific OTLP endpoint; takes precedence over the generic endpoint for trace export. |
| `OTEL_EXPORTER_OTLP_METRICS_ENDPOINT` | All runtime services | Optional metrics-specific OTLP endpoint; takes precedence over the generic endpoint for metrics export. |
| `OTEL_EXPORTER_OTLP_LOGS_ENDPOINT` | All runtime services | Optional logs-specific OTLP endpoint; takes precedence over the generic endpoint for log export. |
| `OTEL_SERVICE_NAME` | All runtime services | Optional OpenTelemetry service name; otherwise the service supplies its own default. |
| `OTEL_RESOURCE_ATTRIBUTES` | All runtime services | Optional comma-separated OpenTelemetry resource attributes. `service.instance.id`, `service.name`, and `process.pid` are set or defaulted by the application. |
| `CONTAINER_APP_NAME` | DataPodSilo in Azure Container Apps | Platform-provided app name used as the Silo advertised hostname when the `AzureContainer` profile is active. |


## App Configuration schema

The providers load unlabelled keys, then keys labelled with the selected environment (`DOTNET_ENVIRONMENT`, otherwise `ASPNETCORE_ENVIRONMENT`, otherwise `Production`). When `KUSDEPOT_DEPLOYMENT_PROFILE=AzureContainer`, profile-labelled keys are loaded last and override matching earlier keys.


| Key | Consumer | Requirement / shape |
|---|---|---|
| `Orleans:ClusterId` | Catalog, DataControl, DataPodSilo | Cluster identifier. |
| `Orleans:ServiceId` | Catalog, DataControl, DataPodSilo | Service identifier. |
| `Orleans:AzureTableEndpoint` | Catalog, DataControl, DataPodSilo | Required for the `AzureContainer` profile; absolute HTTPS Azure Tables endpoint used for clustering. |
| `Catalog:Port` | Catalog | HTTP listen port. |
| `Catalog:GatewayPort` | Catalog | Orleans client gateway port for local clustering. |
| `Catalog:Serilog:MinimumLevel` | Catalog | Optional Serilog level name. |
| `Catalog:KusDepotExceptions:Enabled` | Catalog | Optional Boolean controlling detailed application exception behavior. |
| `DataControl:Port` | DataControl | HTTP listen port. |
| `DataControl:GatewayPort` | DataControl | Orleans client gateway port for local clustering. |
| `DataControl:Serilog:MinimumLevel` | DataControl | Optional Serilog level name. |
| `DataControl:KusDepotExceptions:Enabled` | DataControl | Optional Boolean controlling detailed application exception behavior. |
| `DataControl:Tracing:Enabled` | DataControl | Optional Boolean to enable DataControl activity creation. |
| `DataControl:Database:ConnectionString` | DataControl | Optional PostgreSQL connection string. Its database is used when `Name` is omitted; if both specify a database, they must match. If absent, compose the connection using the address, port, and effective name below. |
| `DataControl:Database:Address` | DataControl | PostgreSQL host when no connection string is supplied. |
| `DataControl:Database:Port` | DataControl | PostgreSQL port when no connection string is supplied. |
| `DataControl:Database:Name` | DataControl | Optional PostgreSQL database name. When omitted, use the database in `ConnectionString`, or `datacontrol` if neither specifies one. |
| `DataControl:SignalREndpoint` | DataControl | Optional absolute HTTPS Azure SignalR endpoint. If absent, SignalR uses its non-Azure/default configuration. |
| `DataControl:TransferConfiguration:DefaultSegmentSizePolicy:MinSegmentBytes` | DataControl | Optional positive lower bound for segment size. |
| `DataControl:TransferConfiguration:DefaultSegmentSizePolicy:PreferredSegmentBytes` | DataControl | Optional preferred size. |
| `DataControl:TransferConfiguration:DefaultSegmentSizePolicy:MaxSegmentBytes` | DataControl | Optional positive upper bound. |
| `DataControl:TransferConfiguration:PayloadRateLimits:UploadBytesPerSecond` | DataControl | Optional non-negative maximum aggregate application-payload upload pacing rate in bytes per second per replica. It does not guarantee end-to-end or network throughput; storage and other delays can lower the observed rate. `0` (the default) means unlimited. |
| `DataControl:TransferConfiguration:PayloadRateLimits:DownloadBytesPerSecond` | DataControl | Optional non-negative maximum aggregate application-payload download pacing rate in bytes per second per replica. It does not guarantee end-to-end or network throughput; processing and other delays can lower the observed rate. `0` (the default) means unlimited. |
| `DataControl:TransferConfiguration:Enforcement:EnforceMinimumSegmentBytes` | DataControl | Optional Boolean. |
| `DataControl:TransferConfiguration:Enforcement:EnforceMaximumSegmentBytes` | DataControl | Optional Boolean. |
| `DataControl:Recovery:PollingInterval` | DataControl | Optional positive interval between publication recovery scans by the elected leader; defaults to 10 minutes. |
| `DataControl:Recovery:LeaderRetryInterval` | DataControl | Optional positive maximum randomized delay before a follower retries publication recovery leadership; defaults to 30 seconds. |
| `DataControl:Recovery:InitialJitterMaximum` | DataControl | Optional non-negative initial jitter. |
| `DataControl:Recovery:CycleJitterMaximum` | DataControl | Optional non-negative cycle jitter. |
| `DataControl:Recovery:CandidatePageSize` | DataControl | Optional positive maximum number of recovery candidates fetched per database query; defaults to 50. |
| `DataControl:Recovery:TerminalFailureThreshold` | DataControl | Optional terminal failure threshold. |
| `DataControl:Deletion:PollingInterval` | DataControl | Optional positive interval between scans for due durable deletion jobs by the elected leader; defaults to 10 minutes. |
| `DataControl:Deletion:LeaderRetryInterval` | DataControl | Optional positive maximum randomized delay before a follower retries deletion leadership; defaults to 30 seconds. |
| `DataControl:Deletion:InitialJitterMaximum` | DataControl | Optional non-negative startup jitter before deletion-job scans; defaults to 2 minutes. |
| `DataControl:Deletion:CycleJitterMaximum` | DataControl | Optional non-negative random delay added after each deletion-job scan; defaults to 1 minute. |
| `DataControl:Deletion:RetryBaseDelay` | DataControl | Optional positive base duration for exponential deletion retry backoff; defaults to 5 seconds. |
| `DataControl:Deletion:MaximumRetryDelay` | DataControl | Optional maximum duration for deletion retry backoff; defaults to 15 minutes. |
| `DataControl:Deletion:CandidatePageSize` | DataControl | Optional positive maximum number of due deletion jobs loaded per scan; defaults to 50. |
| `DataControl:Cleanup:InactivityTimeout` | DataControl | Optional positive inactivity age before a transfer session can be considered for cleanup; defaults to 24 hours. |
| `DataControl:Cleanup:PollingInterval` | DataControl | Optional positive interval between transfer-session cleanup scans by the elected leader; defaults to 15 minutes. |
| `DataControl:Cleanup:LeaderRetryInterval` | DataControl | Optional positive maximum randomized delay before a follower retries cleanup leadership; defaults to 30 seconds. |
| `DataControl:Cleanup:InitialJitterMaximum` | DataControl | Optional non-negative randomized delay before the first cleanup scan; defaults to 2 minutes. |
| `DataControl:Cleanup:CandidatePageSize` | DataControl | Optional positive maximum number of stale transfer-session candidates loaded per database page; defaults to 50. |
| `DataPodSilo:IPAddress` | DataPodSilo | Required non-empty IP address for local Orleans endpoint configuration. |
| `DataPodSilo:SiloPort` | DataPodSilo | Required positive Orleans Silo port. |
| `DataPodSilo:GatewayPort` | DataPodSilo | Required positive Orleans gateway port. |
| `DataPodSilo:ListenOnAnyHostAddress` | DataPodSilo | Optional Boolean. |
| `DataPodSilo:Serilog:MinimumLevel` | DataPodSilo | Optional Serilog level name. |
| `DataPodSilo:KusDepotExceptions:Enabled` | DataPodSilo | Optional Boolean controlling detailed application exception behavior. |
| `CatalogDB:Database:Address` | DataPodSilo and database tooling | PostgreSQL host for CatalogDB connections. |
| `CatalogDB:Database:Port` | DataPodSilo | PostgreSQL port for CatalogDB connections. |
| `CatalogDB:Serilog:MinimumLevel` | DataPodSilo | Optional Serilog level name for CatalogDB logging. |
| `DataConfiguration:Serilog:MinimumLevel` | DataPodSilo | Optional Serilog level name for DataConfiguration logging. |
| `DataConfiguration:Tracing:Enabled` | DataPodSilo | Optional Boolean to enable DataConfiguration activity creation. |
| `CatalogDB:Tracing:Enabled` | DataPodSilo | Optional Boolean to enable CatalogDB activity creation. |

## Key Vault schema

The runtime identity requires permission to read the configured secrets:

| Secret name | Purpose |
|---|---|
| `DataPod--Postgres--PGUSER` | Optional PostgreSQL username. |
| `DataPod--Postgres--PGPASSWORD` | Optional PostgreSQL password. |
| `DataPod--Admin--TenantID` | Required DataConfiguration administrator tenant ID. |
| `DataPod--Admin--AppClientID` | Required DataConfiguration administrator application client ID. |
| `DataPod--Catalogs--{catalog}--TenantID` | Tenant ID for a configured catalog identity. |
| `DataPod--Catalogs--{catalog}--AppClientID` | Application client ID for a configured catalog identity. |
| `DataPod--Catalogs--{catalog}--ConnectionString` | Database connection string for that catalog. |

For PostgreSQL authentication, configure both PostgreSQL username/password secrets or neither: when absent, the service uses Microsoft Entra authentication. Each catalog requires all three catalog secrets.
