namespace KusDepot.FabricExams;

internal static class DataGenerator
{
    public static String GenerateUnicodeString(Int32 length)
    {
        StringBuilder _s = new StringBuilder(length);
        Byte[] _b = new Byte[4];

        using(RandomNumberGenerator _r = RandomNumberGenerator.Create())
        {
            for(Int32 i = 0; i < length; i++)
            {
                Int32 _cp;
                do
                {
                    _r.GetBytes(_b);
                    _cp = BitConverter.ToInt32(_b, 0) & 0x10FFFF;
                }
                while(!IsValidCodePoint(_cp));

                _s.Append(Char.ConvertFromUtf32(_cp));
            }
        }

        return _s.ToString();
    }

    private static Boolean IsValidCodePoint(Int32 cp)
    {
        return (cp < 0xD800 || cp > 0xDFFF) && (cp & 0xFFFE) != 0xFFFE;
    }
}