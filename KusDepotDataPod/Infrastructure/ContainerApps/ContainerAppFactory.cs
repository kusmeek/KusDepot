namespace KusDepotDataPod.Infrastructure;

internal static class ContainerAppFactory
{
    public static ContainerApp Create(
        ContainerAppProfile profile,
        DataPodConfiguration config,
        ResourceGroup resourceGroup,
        ManagedEnvironment environment)
    {
        var additionalPortMappings = new InputList<IngressPortMappingArgs>();

        if(profile.AdditionalPortMappings is not null)
        {
            additionalPortMappings.AddRange(profile.AdditionalPortMappings);
        }

        return new ContainerApp(profile.LogicalName, new()
        {
            ResourceGroupName = resourceGroup.Name,
            ContainerAppName = profile.AppName,
            Location = config.Location,
            Identity = new ManagedServiceIdentityArgs
            {
                Type = "UserAssigned",
                UserAssignedIdentities = { config.IdentityResourceId }
            },
            ManagedEnvironmentId = environment.Id,
            Configuration = new ConfigurationArgs
            {
                ActiveRevisionsMode = "Single",
                Registries =
                {
                    new RegistryCredentialsArgs
                    {
                        Server = config.RegistryLoginServer,
                        Identity = config.IdentityResourceId
                    }
                },
                Secrets =
                {
                    new SecretArgs
                    {
                        Name = "appinsights-connection-string",
                        Value = config.ApplicationInsightsConnectionString
                    }
                },
                Ingress = new IngressArgs
                {
                    AllowInsecure = false,
                    External = profile.IngressExternal,
                    TargetPort = profile.IngressTargetPort,
                    Transport = profile.IngressTransport,
                    AdditionalPortMappings = additionalPortMappings
                }
            },
            Template = new TemplateArgs
            {
                Containers =
                {
                    new ContainerArgs
                    {
                        Name = profile.AppName,
                        Image = profile.Image,
                        Env =
                        {
                            new EnvironmentVarArgs
                            {
                                Name = "KUSDEPOT_APPCONFIGURATION_ENDPOINT",
                                Value = config.AppConfigurationEndpoint
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "KUSDEPOT_KEYVAULT_URI",
                                Value = config.KeyVaultUri
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "KUSDEPOT_DEPLOYMENT_PROFILE",
                                Value = "AzureContainer"
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "AZURE_CLIENT_ID",
                                Value = config.IdentityClientId
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "OTEL_TRACES_SAMPLER",
                                Value = "microsoft.fixed_percentage"
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "OTEL_TRACES_SAMPLER_ARG",
                                Value = "1.0"
                            },
                            new EnvironmentVarArgs
                            {
                                Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                                SecretRef = "appinsights-connection-string"
                            }
                        },
                        Resources = new ContainerResourcesArgs
                        {
                            Cpu = 0.25,
                            Memory = "0.5Gi"
                        },
                        Probes = ContainerAppProbes.Create(profile)
                    }
                },
                Scale = new ScaleArgs
                {
                    MinReplicas = profile.MinReplicas,
                    MaxReplicas = profile.MaxReplicas
                }
            }
        });
    }
}
