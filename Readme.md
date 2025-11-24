[![Build Status](https://dev.azure.com/kusmeek/KusDepot/_apis/build/status/kusmeek.KusDepot?branchName=main)](https://dev.azure.com/kusmeek/KusDepot/_build/latest?definitionId=1&branchName=main)

<div align="center">
  <h1>KusDepot Solution</h1>
</div>

<br>

The Tool is a foundation for business workflows. It processes CommandWorkflows by executing Commands, defined by specialized compartmentalized functionality, as well as hosting and managing itself and other IHostedServices.
Configuration, Data, Logger, Status, and WorkingSet are available, as well as a customizable Access Manager and Dependency Injection Container.

For integration see ToolConnect, DependencyInjection, KusDepotRegistry, SecurityKeyWeb, CommandDetailsWeb, and KusDepotCab.


Storage items include TextItem, CodeItem, BinaryItem, MultiMediaItem, MSBuildItem, GenericItem, KeySet, and DataSetItem.
The GuidReferenceItem stores references and graphs.


Program units implement IHost and integrate the respective application types - ToolWebHost, ToolGenericHost, and ToolAspireHost.

Each has a builder and can be composed with the others to form arbitrarily complex services.

See DaprSidecar, DaprActors, ToolService, ToolActor, ToolGrain, ToolWorkflow, and MCPTool.

DataControl and Catalog APIs manage item storage and searching using Fabric Actors.

<br>

---

<br>
<div align="center">
  <h2>Tool &amp; Data Items</h2>  
  <a href="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/Objects.png" target="_blank">
     <img src="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/Objects.png" alt="Tool & Data Items" style="max-width:100%; height:auto;" />
  </a>  
</div>
<br>

---

<br>
<div align="center">
  <h2>Builders</h2>
  <a href="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/Builders.png" target="_blank">
     <img src="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/Builders.png" alt="Builders" style="max-width:100%; height:auto;" />
  </a>
</div>
<br>

---

<br>
<div align="center">
  <h2>Program Units</h2>
  <a href="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/ProgramUnits.png" target="_blank">
     <img src="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/ProgramUnits.png" alt="Program Units" style="max-width:100%; height:auto;" />
  </a>
</div>
<br>

---

<br>
<div align="center">
  <h2>Data Fabric Actor Services</h2>
  <a href="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/ServiceInterfaces.png" target="_blank">
     <img src="https://raw.githubusercontent.com/kusmeek/KusDepot/main/KusDepot.Docs/Diagrams/ServiceInterfaces.png" alt="Data Fabric Actor Services" style="max-width:100%; height:auto;" />
  </a>
</div>
<br>

---

<br>

Powered by Microsoft Entra and Azure Service Fabric. For more information see KusDepot.Docs.

Copyright © 2025 Michael Abrahams <mike_abrahams@outlook.com>

Graphic background from https://jooinn.com/starfield.html