﻿global using System;
global using Serilog;
global using DataControl;
global using KusDepot.Data;
global using System.Fabric;
global using System.Threading;
global using System.Text.Json;
global using System.Diagnostics;
global using OpenTelemetry.Trace;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.OpenApi.Models;
global using static System.Environment;
global using Microsoft.AspNetCore.Http;
global using System.Collections.Generic;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.ServiceFabric.Actors;
global using static KusDepot.Data.TraceUtility;
global using Microsoft.AspNetCore.Http.Features;
global using Microsoft.Extensions.Configuration;
global using Microsoft.ServiceFabric.Actors.Client;
global using static KusDepot.Data.DataControlUtility;
global using static KusDepot.Data.DataControlStrings;
global using Microsoft.ServiceFabric.Services.Runtime;
global using Microsoft.Extensions.DependencyInjection;
global using DiagnosticActivity = System.Diagnostics.Activity;
global using Microsoft.ServiceFabric.Services.Communication.Runtime;