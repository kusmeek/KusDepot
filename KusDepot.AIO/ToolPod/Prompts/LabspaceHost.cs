namespace KusDepot.AI;

[McpServerPromptType]
[Description("MCP prompts that guide callers through ToolPod workflows using the KusDepot Tool framework.")]
public static class ToolPodPrompts
{
    [McpServerPrompt(Name = "ToolPodLabspaceDemo")]
    [Description("Generates a guided walkthrough that demonstrates the KusDepot Tool framework using the Labspace assembly. Covers creating a LabHost, adding hosted services (CounterService, EchoService, ConfigurableService), registering and executing a MathCommand, and full lifecycle management — all through ToolPod.")]
    public static IList<PromptMessage> LabspaceDemo(
        [Description("Absolute directory path where Labspace.dll is located.")] String assemblyDirectory)
    {
        List<PromptMessage> messages = new();

        messages.Add(new()
        {
            Role = Role.User,
            Content = new TextContentBlock
            {
                Text = "Show me a full demo of the KusDepot Tool framework using the Labspace assembly. " +
                       "I want to see dynamic service hosting, command registration and execution, reference passing between objects, and lifecycle management — all driven through ToolPod."
            }
        });

        messages.Add(new()
        {
            Role = Role.Assistant,
            Content = new TextContentBlock
            {
                Text = $"""
                Here is a complete guided walkthrough of the Labspace types using ToolPod. Follow each step in order — I will explain what each call demonstrates.

                ---

                ## Phase 1 — Load the assembly

                1. **ToolPodSetWorkingDirectory** ? path = `{assemblyDirectory}`
                2. **ToolPodLoadAssembly** ? fileName = `Labspace.dll`
                3. **ToolPodListTypes** ? assemblyName = `Labspace`
                   - Confirm the exported types: `LabHost`, `CounterService`, `EchoService`, `ConfigurableService`, `LabConfig`, `MathCommand`.

                ---

                ## Phase 2 — Create the host

                4. **ToolPodCreate** ? type = `KusDepot.Test.LabHost, Labspace`, alias = `host`
                   - This creates a `Tool` that acts as the hosting root for services and commands.
                5. **ToolPodDescribe** ? idoralias = `host`
                   - Observe the lifecycle, hosting, and command methods inherited from `Tool`.

                ---

                ## Phase 3 — Add hosted services dynamically

                Each service is added with a `name` so the host can look them up via `GetHostedServiceNames` and `GetHostedService(name)`.
                The `AddHostedService` signature is `(service, name, permissions, request, start)` — pass null for `permissions` and `request`.

                **CounterService** — a stateful counter with Increment, Decrement, Reset.

                6. **ToolPodCreate** ? type = `KusDepot.Test.CounterService, Labspace`, alias = `counter`
                7. **ToolPodInvoke** ? target = `host`, method = `AddHostedService`, arguments:
                   - [0] Kind=Reference, RefId=`counter` (the ITool service)
                   - [1] Kind=Value, Data=`counter`, Type=`System.String` (name)
                   - [2] Kind=Value, Data=null, Type=null (permissions — skip)
                   - [3] Kind=Value, Data=null, Type=null (request — skip)
                   - [4] Kind=Value, Data=`true`, Type=`System.Boolean` (start)

                **EchoService** — a message store with Post, Clear.

                8. **ToolPodCreate** ? type = `KusDepot.Test.EchoService, Labspace`, alias = `echo`
                9. **ToolPodInvoke** ? target = `host`, method = `AddHostedService`, arguments:
                    - [0] Kind=Reference, RefId=`echo`
                    - [1] Kind=Value, Data=`echo`, Type=`System.String` (name)
                    - [2] Kind=Value, Data=null, Type=null (permissions — skip)
                    - [3] Kind=Value, Data=null, Type=null (request — skip)
                    - [4] Kind=Value, Data=`true`, Type=`System.Boolean` (start)

                **ConfigurableService** — demonstrates reference passing. Requires a `LabConfig` constructor argument.

                10. **ToolPodCreate** ? type = `KusDepot.Test.LabConfig, Labspace`, alias = `cfg`, arguments:
                    - [0] Kind=Value, Data=`DemoConfig`, Type=`System.String`
                    - [1] Kind=Value, Data=`50`, Type=`System.Int32`
                    - [2] Kind=Value, Data=`true`, Type=`System.Boolean`
                11. **ToolPodCreate** ? type = `KusDepot.Test.ConfigurableService, Labspace`, alias = `configsvc`, arguments:
                    - [0] Kind=Reference, RefId=`cfg`
                12. **ToolPodInvoke** ? target = `host`, method = `AddHostedService`, arguments:
                    - [0] Kind=Reference, RefId=`configsvc`
                    - [1] Kind=Value, Data=`configsvc`, Type=`System.String` (name)
                    - [2] Kind=Value, Data=null, Type=null (permissions — skip)
                    - [3] Kind=Value, Data=null, Type=null (request — skip)
                    - [4] Kind=Value, Data=`true`, Type=`System.Boolean` (start)

                ---

                ## Phase 4 — Start the host and verify lifecycle

                13. **ToolPodInvoke** ? target = `host`, method = `StartHostAsync`
                    - The host transitions to `Active` and starts all hosted services.
                14. **ToolPodGetProperty** ? target = `host`, name = `Status`
                    - Expect `Active`.
                15. **ToolPodInvoke** ? target = `host`, method = `GetHostedServiceNames`
                    - Expect `["counter", "echo", "configsvc"]`.

                ---

                ## Phase 5 — Exercise the services

                **Counter:**
                16. **ToolPodInvoke** ? target = `counter`, method = `Increment`
                17. **ToolPodInvoke** ? target = `counter`, method = `Increment`
                18. **ToolPodGetProperty** ? target = `counter`, name = `Count` — expect `2`.

                **Echo:**
                19. **ToolPodInvoke** ? target = `echo`, method = `Post`, arguments:
                    - [0] Kind=Value, Data=`Hello from ToolPod`, Type=`System.String`
                20. **ToolPodInvoke** ? target = `echo`, method = `Post`, arguments:
                    - [0] Kind=Value, Data=`Second message`, Type=`System.String`
                21. **ToolPodGetProperty** ? target = `echo`, name = `MessageCount` — expect `2`.
                22. **ToolPodGetProperty** ? target = `echo`, name = `Messages` — expect the array.

                **ConfigurableService:**
                23. **ToolPodGetProperty** ? target = `configsvc`, name = `ConfigName` — expect `DemoConfig`.
                24. **ToolPodGetProperty** ? target = `configsvc`, name = `ConfigMaxItems` — expect `50`.
                25. **ToolPodGetProperty** ? target = `configsvc`, name = `ConfigVerbose` — expect `true`.

                ---

                ## Phase 6 — Register and execute a command

                26. **ToolPodCreate** ? type = `KusDepot.Test.MathCommand, Labspace`, alias = `math`
                27. **ToolPodInvoke** ? target = `host`, method = `RegisterCommand`, arguments:
                    - [0] Kind=Value, Data=`Multiply`, Type=`System.String` (the handle)
                    - [1] Kind=Reference, RefId=`math`
                28. **ToolPodInvoke** ? target = `host`, method = `Activate`
                29. **ToolPodInvoke** ? target = `host`, method = `EnableAllCommands`

                Build a `CommandDetails` using the static `Create()` factory and fluent instance methods.
                **Do not** set properties directly — `CommandDetails` uses set-once semantics (`??=`), so `SetProperty` calls after construction will silently fail. Use `ToolPodInvokeStatic` to call `Create`, then chain `SetHandle` / `SetArgument` via **ToolPodInvoke**:

                30. **ToolPodInvokeStatic** ? type = `KusDepot.CommandDetails, KusDepot.Objects`, method = `Create`, alias = `cmd`
                    - This returns a new empty `CommandDetails` tracked as `cmd`.
                31. **ToolPodInvoke** ? target = `cmd`, method = `SetHandle`, arguments:
                    - [0] Kind=Value, Data=`Multiply`, Type=`System.String`
                    - Returns the same `CommandDetails` (fluent).
                32. **ToolPodInvoke** ? target = `cmd`, method = `SetArgument`, arguments:
                    - [0] Kind=Value, Data=`Operation`, Type=`System.String`
                    - [1] Kind=Value, Data=`Multiply`, Type=`System.String`
                33. **ToolPodInvoke** ? target = `cmd`, method = `SetArgument`, arguments:
                    - [0] Kind=Value, Data=`x`, Type=`System.String`
                    - [1] Kind=Value, Data=`6.0`, Type=`System.Double`
                34. **ToolPodInvoke** ? target = `cmd`, method = `SetArgument`, arguments:
                    - [0] Kind=Value, Data=`y`, Type=`System.String`
                    - [1] Kind=Value, Data=`7.0`, Type=`System.Double`

                Execute:
                35. **ToolPodInvoke** ? target = `host`, method = `ExecuteCommandAsync`, arguments:
                    - [0] Kind=Reference, RefId=`cmd`
                    - This returns a `Guid?` activity ID.
                36. **ToolPodInvoke** ? target = `host`, method = `GetOutput`, arguments:
                    - [0] the activity ID from step 35
                    - Expect result `42.0`.

                ---

                ## Phase 7 — Tear down

                37. **ToolPodInvoke** ? target = `host`, method = `StopHostAsync`
                38. **ToolPodGetProperty** ? target = `host`, name = `Status` — expect `InActive`.
                39. **ToolPodUnloadAssemblies** ? reset the entire ToolPod context.

                ---

                This walkthrough exercises **dynamic service hosting**, **constructor reference passing**, **command registration and execution with fluent CommandDetails construction**, **lifecycle management**, and **stateful object interaction** — all through the ToolPod MCP surface.

                Shall I begin with Phase 1?
                """
            }
        });

        return messages;
    }
}
