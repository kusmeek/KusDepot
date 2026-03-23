global using KusDepot.Data.Models;
global using KusDepot.Data.Services.Configuration;
global using KusDepot.Security;
global using Microsoft.ServiceFabric.Actors;
global using Microsoft.ServiceFabric.Actors.Client;
global using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
global using Microsoft.ServiceFabric.Services.Remoting;
global using System;
global using System.Collections.Generic;
global using System.Fabric;
global using System.Threading;
global using System.Threading.Tasks;
[assembly: FabricTransportActorRemotingProvider(RemotingClientVersion = RemotingClientVersion.V2_1,RemotingListenerVersion = RemotingListenerVersion.V2_1)]