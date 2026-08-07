Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.ToolWorkflow" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.ToolWorkflow" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "fabric:/KusDepot.ToolWorkflowType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Get-ServiceFabricService -ApplicationName "fabric:/KusDepot.Dap" | Remove-ServiceFabricService -Force -ForceRemove -Verbose

Get-ServiceFabricApplication -ApplicationName "fabric:/KusDepot.Dap" | Remove-ServiceFabricApplication -Force -Verbose

Get-ServiceFabricApplicationType -ApplicationTypeName "KusDepot.DapType" | Unregister-ServiceFabricApplicationType -Force -Verbose

Connect-ServiceFabricCluster