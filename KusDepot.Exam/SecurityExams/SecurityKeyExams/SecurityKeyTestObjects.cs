namespace KusDepot.Exams;

public sealed class SecurityKeyTest : SecurityKey
{
    public SecurityKeyTest() {}

    public override SecurityKeyTest? Clone()
    {
        return Parse<SecurityKeyTest>(this.ToString(),null);
    }
}