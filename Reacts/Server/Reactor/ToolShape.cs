namespace KusDepot.Reacts;

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