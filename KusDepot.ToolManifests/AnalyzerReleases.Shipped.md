## Release 0.0.8-pre

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
KDT001 | KusDepot.ToolManifests | Error | ToolManifest target must derive from KusDepot.Tool
KDT002 | KusDepot.ToolManifests | Error | Generic tool types are not supported
KDT003 | KusDepot.ToolManifests | Error | Custom tool operation method names must resolve to one protected-operation index
KDT004 | KusDepot.ToolManifests | Error | Custom operation index is out of valid range
KDT005 | KusDepot.ToolManifests | Error | ToolManifest target must declare ToolSchemaID
KDT006 | KusDepot.ToolManifests | Warning | Sync and async tool operation pairs should share the same protected-operation index
KDT007 | KusDepot.ToolManifests | Error | Handwritten manifest descriptor index must match AccessCheck index
KDT008 | KusDepot.ToolManifests | Error | Handwritten manifest descriptor must match a real tool operation method
KDT009 | KusDepot.ToolManifests | Error | CompanionNamespace must be a valid namespace
KDT010 | KusDepot.ToolManifests | Error | Assembly CompanionNamespace must be a valid namespace
KDT011 | KusDepot.ToolManifests | Error | CompanionTypeName must be a valid type name
