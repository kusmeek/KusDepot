global using System;
global using KusDepot;
global using System.Xml;
global using System.Linq;
global using System.Fabric;
global using System.Reflection;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Security.Cryptography;
global using Microsoft.ServiceFabric.Actors;
global using Microsoft.IdentityModel.JsonWebTokens;
global using Microsoft.ServiceFabric.Actors.Client;
global using static System.Globalization.CultureInfo;
global using Microsoft.ServiceFabric.Services.Remoting;
global using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
[assembly: FabricTransportActorRemotingProvider(RemotingClientVersion = RemotingClientVersion.V2_1 , RemotingListenerVersion = RemotingListenerVersion.V2_1)]