namespace DataPodServices.CatalogDB;

public static class CatalogPageTokenCodec
{
    private const Byte CurrentVersion = 1;

    private const Int32 PayloadLength = 17;

    private const Int32 TokenLength = 23;

    public static String Encode(Guid key)
    {
        Span<Byte> payload = stackalloc Byte[PayloadLength];

        payload[0] = CurrentVersion;

        if(!key.TryWriteBytes(payload[1..],bigEndian:true,out _))
        {
            throw new InvalidOperationException("Unable to encode the catalog page key.");
        }

        return Convert.ToBase64String(payload)
            .TrimEnd('=')
            .Replace('+','-')
            .Replace('/','_');
    }

    public static Guid? Decode(String? token)
    {
        if(token is null) { return null; }

        if(token.Length != TokenLength)
        {
            throw new CatalogPageRequestException("CatalogPageToken has an invalid format.");
        }

        foreach(Char character in token)
        {
            if(!(character is >= 'A' and <= 'Z') &&
               !(character is >= 'a' and <= 'z') &&
               !(character is >= '0' and <= '9') &&
               character is not '-' and not '_')
            {
                throw new CatalogPageRequestException("CatalogPageToken has an invalid format.");
            }
        }

        String base64 = token.Replace('-','+').Replace('_','/') + "=";

        Byte[] payload;

        try
        {
            payload = Convert.FromBase64String(base64);
        }
        catch ( FormatException exception )
        {
            throw new CatalogPageRequestException($"CatalogPageToken has an invalid format: {exception.Message}");
        }

        if(payload.Length != PayloadLength)
        {
            throw new CatalogPageRequestException("CatalogPageToken has an invalid format.");
        }

        if(payload[0] != CurrentVersion)
        {
            throw new CatalogPageRequestException("CatalogPageToken uses an unsupported version.");
        }

        Guid key = new(payload.AsSpan(1),bigEndian:true);

        if(!String.Equals(Encode(key),token,StringComparison.Ordinal))
        {
            throw new CatalogPageRequestException("CatalogPageToken has an invalid format.");
        }

        return key;
    }
}
