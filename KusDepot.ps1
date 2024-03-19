Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.ToolServiceFabricType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.ToolServiceHostType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.ToolServiceFabric" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.ToolServiceHost" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.ToolServiceFabric" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.ToolServiceHost" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.DataType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Data" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Data" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.ActType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Act" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Act" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Add-Type -AssemblyName ($env:KusDepotSolution + "\KusDepot.Objects\bin\Debug\net7.0\KusDepot.Objects.dll") -Verbose

Import-Module ($env:KusDepotSolution + "\CmdletPS\bin\Debug\net7.0\CmdletPS.dll") -Verbose

&($env:KusDepotSolution + "\KusDepot.ToolService\bin\x64\Debug\net7.0\ToolService.exe");

&($env:KusDepotSolution + "\Command\bin\Debug\net7.0\Command.exe") Multiply 246.8 135.7

&($env:KusDepotSolution + "\Laboratory\bin\x64\Debug\net7.0\Laboratory.exe");

&($env:KusDepotSolution + "\Labnativemix\bin\x64\Debug\Labnativemix.exe");

&($env:KusDepotSolution + "\WebTool\bin\x64\Debug\net7.0\WebTool.exe");

&($env:KusDepotSolution + "\View\bin\Debug\net7.0-Windows\View.exe");

Invoke-Tool @("Multiply",$null,50,200)

[KusDepot.Tool]::new().GetID()

Connect-ServiceFabricCluster