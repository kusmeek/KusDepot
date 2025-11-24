Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.ToolWorkflow" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.ToolWorkflow" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "fabric:/KusDepot.ToolWorkflowType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.ToolServiceFabric" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.ToolServiceFabric" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.ToolServiceFabricType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.ToolServiceHost" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.ToolServiceHost" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.ToolServiceHostType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Orleans" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Orleans" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.OrleansType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Weblzr" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Weblzr" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.WeblzrType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Data" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Data" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.DataType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Dap" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Dap" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.DapType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KustoEmulator" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KustoEmulator" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KustoEmulatorType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Act" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Act" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.ActType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Connect-ServiceFabricCluster