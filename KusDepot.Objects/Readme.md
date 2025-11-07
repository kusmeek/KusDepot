[![Build Status](https://dev.azure.com/kusmeek/KusDepot/_apis/build/status/kusmeek.KusDepot?branchName=main)](https://dev.azure.com/kusmeek/KusDepot/_build/latest?definitionId=1&branchName=main)

KusDepot Solution.

The Tool is a foundation for business workflows. It stores DataItems and hosts Services and Commands, which define
specialized compartmentalized functionality. Configuration, Inputs, Outputs, Logger, Status, and WorkingSet are
available, as well as an Access Manager, Quartz Scheduler, and Autofac Container.

Storage items include TextItem, CodeItem, BinaryItem, MultiMediaItem, MSBuildItem, GenericItem, and DataSetItem.
The GuidReferenceItem stores references to all the item types and can be used to form arbitrary graphs.

Program units derive from IHost and include ToolHost, ToolWebHost, ToolGenericHost, and ToolAspireHost. Each has
a builder and can be composed with the others to form arbitrarily complex services. See DaprSidecar, DaprActors,
ToolService, ToolActor, ToolGrain, ToolWorkflow, and MCPTool. For integration see KusDepotRegistry,
DependencyInjection, ToolConnect, SecurityKeyWeb, CommandDetailsWeb, and KusDepotCab.

Powered by Microsoft Entra and Azure Service Fabric.

For more information see KusDepot.Docs.

Copyright © 2025 Michael Abrahams <mike_abrahams@outlook.com>

Graphic background from https://jooinn.com/starfield.html