global using System;
global using System.Xml;
global using System.Linq;
global using System.Fabric;
global using KusDepot.Services;
global using KusDepot.Data.Models;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using Microsoft.ServiceFabric.Actors;
global using Microsoft.ServiceFabric.Actors.Client;
global using Microsoft.IdentityModel.JsonWebTokens;
global using static System.Globalization.CultureInfo;
global using Microsoft.ServiceFabric.Services.Remoting;
global using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
[assembly: FabricTransportActorRemotingProvider(RemotingClientVersion = RemotingClientVersion.V2_1,RemotingListenerVersion = RemotingListenerVersion.V2_1)]