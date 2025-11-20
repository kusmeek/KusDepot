namespace Labvoid;

internal static class Space
{
    private static void SetupLog()
    { 
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
    }

    private static Double Random(Double min , Double max)
    {
        var b = new Byte[8]; RandomNumberGenerator.Fill(b);

        return (BitConverter.ToUInt64(b,0) / (Double)UInt64.MaxValue) * (max - min) + min;
    }

    internal static void Main()
    {
        SetupLog();

        var r = BinaryItem.FromFile(AppContext.BaseDirectory + @"\kusdepot_reactr.bin");

        File.WriteAllBytes(AppContext.BaseDirectory + @"\kusdepot_reactr.dll",r!.GetContent()!);

        while(true)
        {
            Log.Information(GenerateShape(new ToolShape { id = Guid.NewGuid() , x = Random(0,7648) , y = Random(0,4304) })!.ToString());

            Thread.Sleep(1500);
        }
    }
}