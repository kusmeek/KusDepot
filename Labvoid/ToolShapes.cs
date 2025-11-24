namespace ToolShapeServices;

public sealed partial record ToolShape
{
    public Guid?    id       { get; init; }
    public Double?  x        { get; init; }
    public Double?  y        { get; init; }
    public Boolean? circle   { get; init; }
    public Double?  opacity  { get; init; }
    public String?  rgb      { get; init; }
    public Double?  rotation { get; init; }
    public Double?  scale    { get; init; }
    public Int32?   sides    { get; init; }
    public Boolean? star     { get; init; }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ToolShape? AsValid()
    {
        if( id       is null || Equals(Guid.Empty,id) ||
            x        is null || x < 0 ||
            y        is null || y < 0 ||
            circle   is null ||
            opacity  is null || opacity < 0 || opacity > 1      ||
            rgb      is null || Rgb().IsMatch(rgb) is false     ||
            rotation is null || rotation < 0 || rotation > 360  ||
            scale    is null || scale <= 0                      ||
            sides    is null                                    ||
            star     is null
        ) { return null; }

        if(this.circle.Value && this.star.Value) { return null; }

        if(this.circle.Value)
        {
            return Equals(this.sides.Value,0) is true ? this : this with { sides = 0 };
        }

        if(this.sides.Value < 3) { return null; }

        return this;
    }

    [GeneratedRegex("^#[0-9a-fA-F]{6}$")]
    private static partial Regex Rgb();
}

[StructLayout(LayoutKind.Sequential)]
public struct ToolShapeInterOp
{
    [MarshalAs(UnmanagedType.ByValArray,SizeConst = 16)]
    public Byte[] id;
    public Double x;
    public Double y;
    [MarshalAs(UnmanagedType.I1)]
    public Boolean circle;
    public Double opacity;
    [MarshalAs(UnmanagedType.ByValArray,SizeConst = 8)]
    public Byte[] rgb;
    public Double rotation;
    public Double scale;
    public Int32 sides;
    public Boolean star;
}

public static class ToolShapeGenerator
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory)]
    [DllImport("kusdepot_reactr.dll",CallingConvention = CallingConvention.Cdecl)]
    private static extern ToolShapeInterOp generate_shape_interop(ToolShapeInterOp input);

    public static ToolShape? GenerateShape(ToolShape? shape)
    {
        try
        {
            return FromInterOp(generate_shape_interop(ToInterOp(shape)!.Value));
        }
        catch ( Exception _ ) { Log.Error(_,GenerateShapeFail); return null; }
    }

    public static ToolShapeInterOp? ToInterOp(ToolShape? shape)
    {
        try
        {
            if (shape is null)
            {
                return new ToolShapeInterOp
                {
                    id       = new Byte[16],
                    x        = Double.NaN,
                    y        = Double.NaN,
                    circle   = false,
                    opacity  = Double.NaN,
                    rgb      = new Byte[8],
                    rotation = Double.NaN,
                    scale    = Double.NaN,
                    sides    = 0,
                    star     = false
                };
            }
            var idBytes = new Byte[16];
            if (shape.id.HasValue)
            {
                var guidBytes = shape.id.Value.ToByteArray();
                Buffer.BlockCopy(guidBytes, 0, idBytes, 0, 16);
            }
            var rgbBytes = new Byte[8];
            if (!String.IsNullOrEmpty(shape.rgb))
            {
                var bytes = Encoding.UTF8.GetBytes(shape.rgb);
                var len = Math.Min(bytes.Length, 7);
                Buffer.BlockCopy(bytes, 0, rgbBytes, 0, len);
                if (len < 8) rgbBytes[len] = 0;
            }
            return new ToolShapeInterOp
            {
                id       = idBytes,
                x        = shape.x ?? Double.NaN,
                y        = shape.y ?? Double.NaN,
                circle   = shape.circle ?? false,
                opacity  = shape.opacity ?? Double.NaN,
                rgb      = rgbBytes,
                rotation = shape.rotation ?? Double.NaN,
                scale    = shape.scale ?? Double.NaN,
                sides    = shape.sides ?? 0,
                star     = shape.star ?? false
            };
        }
        catch ( Exception _ ) { Log.Error(_,ToInterOpFail); return null; }
    }

    public static ToolShape? FromInterOp(ToolShapeInterOp? interop)
    {
        try
        {
            if (interop is null) return null;
            var value = interop.Value;
            Guid? id = null;
            if (value.id != null && value.id.Length == 16 && Array.Exists(value.id, b => b != 0))
            {
                id = new Guid(value.id);
            }
            Double? x = Double.IsNaN(value.x) ? null : value.x;
            Double? y = Double.IsNaN(value.y) ? null : value.y;
            Boolean? circle = value.circle;
            Double? opacity = Double.IsNaN(value.opacity) ? null : value.opacity;
            String? rgb = null;
            if (value.rgb != null && Array.Exists(value.rgb, b => b != 0))
            {
                Int32 len = Array.IndexOf(value.rgb, (Byte)0);
                if (len < 0) len = 8;
                rgb = Encoding.UTF8.GetString(value.rgb, 0, len);
            }
            Double? rotation = Double.IsNaN(value.rotation) ? null : value.rotation;
            Double? scale = Double.IsNaN(value.scale) ? null : value.scale;
            Int32? sides = value.sides == 0 ? null : value.sides;
            Boolean? star = value.star;
            return new ToolShape
            {
                id       = id,
                x        = x,
                y        = y,
                circle   = circle,
                opacity  = opacity,
                rgb      = rgb,
                rotation = rotation,
                scale    = scale,
                sides    = sides,
                star     = star
            };
        }
        catch ( Exception _ ) { Log.Error(_,FromInterOpFail); return null; }
    }

    public const String GenerateShapeFail = "GenerateShape Failed";
    public const String FromInterOpFail = "FromInterOp Failed";
    public const String ToInterOpFail = "ToInterOp Failed";
}