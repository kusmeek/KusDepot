namespace KusDepot.Reacts;

public static class ToolShapeGenerator
{
    private static readonly RandomNumberGenerator? _rng = RandomNumberGenerator.Create();

    public static ToolShape? GenerateShape(ToolShape? input = null)
    {
        try
        {
            return new ToolShape
            {
                id       = input?.id       ?? Guid.NewGuid(),
                x        = input?.x        ?? RandomCoordinate(7648),
                y        = input?.y        ?? RandomCoordinate(4304),
                circle   = input?.circle   ?? RandomCircle(),
                opacity  = input?.opacity  ?? RandomOpacity(),
                rgb      = input?.rgb      ?? RandomColor(),
                rotation = input?.rotation ?? RandomRotation(),
                scale    = input?.scale    ?? RandomScale(max:2.0),
                sides    = input?.sides    ?? RandomSides(3,8),
                star     = input?.star     ?? RandomStar()
            }.AsValid() ?? GenerateShape(input);
        }
        catch ( Exception _ ) { Log.Logger.Error(_,GenerateShapeFail); return null; }
    }

    private static Boolean RandomCircle()
    {
        try
        {
            var bytes = new Byte[1]; _rng!.GetBytes(bytes); return bytes[0] < 25;
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomCircleFail); return false; }
    }

    private static String RandomColor()
    {
        try
        {
            var bytes = new Byte[3]; _rng!.GetBytes(bytes);

            return $"#{bytes[0]:X2}{bytes[1]:X2}{bytes[2]:X2}";
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomColorFail); return String.Empty; }
    }

    private static Double RandomCoordinate(Int32 max)
    {
        try
        {
            if (max <= 0) { return 0; }

            var bytes = new Byte[4]; _rng!.GetBytes(bytes);

            UInt32 rand = BitConverter.ToUInt32(bytes,0);

            return (rand / (Double)UInt32.MaxValue) * max;
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomCoordinateFail); return 0; }
    }

    private static Double RandomOpacity(Double min = 0.0 , Double max = 1.0)
    {
        try
        {
            var bytes = new Byte[4]; _rng!.GetBytes(bytes);

            UInt32 rand = BitConverter.ToUInt32(bytes,0);

            return (rand / (Double)UInt32.MaxValue) * (max - min) + min;
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomOpacityFail); return 1; }
    }

    private static Double RandomRotation()
    {
        try
        {
            var bytes = new Byte[8]; _rng!.GetBytes(bytes);

            return (BitConverter.ToUInt64(bytes,0) / (Double)UInt64.MaxValue) * 360.0;
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomRotationFail); return 0; }
    }

    private static Double RandomScale(Double min = 0.1 , Double max = 10.0)
    {
        try
        {
            var bytes = new Byte[4]; _rng!.GetBytes(bytes);

            UInt32 rand = BitConverter.ToUInt32(bytes,0);

            return (rand / (Double)UInt32.MaxValue) * (max - min) + min;
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomScaleFail); return 1; }
    }

    private static Int32 RandomSides(Int32 min , Int32 max)
    {
        try
        {
            if(min > max) { return 12; }

            var bytes = new Byte[4]; _rng!.GetBytes(bytes);

            UInt32 rand = BitConverter.ToUInt32(bytes,0);

            return (Int32)(rand % (UInt32)(max - min + 1)) + min;
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomSidesFail); return 12; }
    }

    private static Boolean RandomStar()
    {
        try
        {
            var bytes = new Byte[1]; _rng!.GetBytes(bytes); return bytes[0] < 50;
        }
        catch ( Exception _ ) { Log.Logger.Error(_,RandomStarFail); return false; }
    }
}