namespace LabX02;

internal sealed class LabTextGenerator
{
    private static readonly String[] Prefixes = ["cyan","violet","ember","lattice","echo","ion","quantum","glass","drift","vector"];
    private static readonly String[] Middles = ["pulse","horizon","delta","signal","spiral","orbit","shadow","plasma","thread","beacon"];
    private static readonly String[] Suffixes = ["north","gamma","prime","field","arc","node","crest","wake","frame","spark"];

    private readonly Random random = new();
    private Int64 sequence;

    public String NextLine()
    {
        Int64 current = Interlocked.Increment(ref this.sequence);

        Int32 tokenCount = this.random.Next(3,6);
        List<String> tokens = new(tokenCount);

        for(Int32 i = 0 ; i < tokenCount ; i++)
        {
            Int32 selector = i % 3;

            tokens.Add(selector switch
            {
                0 => Prefixes[this.random.Next(Prefixes.Length)],
                1 => Middles[this.random.Next(Middles.Length)],
                _ => Suffixes[this.random.Next(Suffixes.Length)],
            });
        }

        return $"{current:D6} {DateTimeOffset.Now:HH:mm:ss.fff} {String.Join(' ',tokens)}";
    }
}
