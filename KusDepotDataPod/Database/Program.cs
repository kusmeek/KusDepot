namespace KusDepotDataPod.Database;

internal static class Program
{
    private static async Task<Int32> Main(String[] args)
    {
        if(Arguments.TryParse(args,out Arguments? arguments) is false)
        {
            WriteUsage(); return 2;
        }

        if(arguments!.Help)
        {
            WriteUsage(); return 0;
        }

        try
        {
            IConfiguration configuration = await ConfigurationFactory.CreateAsync(arguments.Operation,arguments.ConfigPath);

            return await DatabaseOperations.RunAsync(arguments,configuration);
        }
        catch ( Exception error )
        {
            Console.Error.WriteLine(error.Message);

            return 1;
        }
    }

    private static void WriteUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  KusDepotDataPod.Database --list-catalogs [--catalog <name>]");
        Console.WriteLine("  KusDepotDataPod.Database --migrate-catalogs [--catalog <name>] [--config <file-path>]");
        Console.WriteLine("  KusDepotDataPod.Database --migrate-datacontrol [--config <file-path>]");
        Console.WriteLine("  KusDepotDataPod.Database --migrate-all [--config <file-path>]");
    }

    internal sealed record Arguments(String Operation , String? Catalog , String? ConfigPath , Boolean Help)
    {
        public static Boolean TryParse(String[] args , out Arguments? arguments)
        {
            arguments = null;

            if(args.Length == 1 && (args[0] is "--help" or "-h"))
            {
                arguments = new Arguments(String.Empty,null,null,true);

                return true;
            }

            if(args.Length < 1) { return false; }

            String operation = args[0] switch
            {
                "--list-catalogs" => "list-catalogs",
                "--migrate-catalogs" => "migrate-catalogs",
                "--migrate-datacontrol" => "migrate-datacontrol",
                "--migrate-all" => "migrate-all",
                _ => String.Empty
            };

            if(String.IsNullOrEmpty(operation)) { return false; }

            String? catalog = null;
            String? configPath = null;

            for(Int32 index = 1; index < args.Length;)
            {
                String option = args[index++];

                if(index >= args.Length || String.IsNullOrWhiteSpace(args[index])) { return false; }

                String value = args[index++];

                switch(option)
                {
                    case "--catalog" when catalog is null:
                        catalog = value;
                        break;

                    case "--config" when configPath is null:
                        configPath = value;
                        break;

                    default:
                        return false;
                }
            }

            if(catalog is not null && (String.IsNullOrWhiteSpace(catalog) || operation is not "list-catalogs" and not "migrate-catalogs")) { return false; }

            if(configPath is not null && operation is not "migrate-catalogs" and not "migrate-datacontrol" and not "migrate-all") { return false; }

            arguments = new Arguments(operation,catalog,configPath,false);

            return true;
        }
    }
}
