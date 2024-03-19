namespace KusDepot;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Compress"]/*'/>*/
    public static Byte[]? Compress(this Byte[]? input)
    {
        try
        {
            if(input is null) { return null; }

            MemoryStream m = new MemoryStream(); GZipStream z = new GZipStream(m,CompressionMode.Compress);

            z.Write(input,0,input.Length); z.Flush(); z.Close(); return m.ToArray();
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Decompress"]/*'/>*/
    public static Byte[]? Decompress(this Byte[]? input)
    {
        try
        {
            if(input is null) { return null; }

            MemoryStream m = new MemoryStream(input); GZipStream z = new GZipStream(m,CompressionMode.Decompress);

            MemoryStream o = new MemoryStream(); z.CopyTo(o); z.Dispose(); return o.ToArray();
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Decrypt"]/*'/>*/
    public static Byte[]? Decrypt(this Byte[]? input , X509Certificate2? certificate)
    {
        try
        {
            if(input == null || certificate == null) { return null; }

            RSA? key = certificate.GetRSAPrivateKey(); if(key is null) { return null; }

            Aes aes = Aes.Create(); aes.KeySize = 256; aes.BlockSize = 128;

            aes.Key = key.Decrypt(input.Take(512).ToArray(),RSAEncryptionPadding.OaepSHA512);

            aes.IV = input.Skip(512).Take(16).ToArray(); aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.ISO10126;

            ICryptoTransform dec = aes.CreateDecryptor(); aes.Dispose(); if(dec is null) { return null; }

            Byte[] data = input.Skip(528).ToArray();

            return dec.TransformFinalBlock(data,0,data.Length);
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DeserializeCertificate"]/*'/>*/
    public static X509Certificate2? DeserializeCertificate(String? certificate)
    {
        try
        {
            if(certificate == null) { return null; }

            return new X509Certificate2(certificate.ToByteArrayFromBase64());
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Encrypt"]/*'/>*/
    public static Byte[]? Encrypt(this Byte[]? input , X509Certificate2? certificate)
    {
        try
        {
            if(input == null || certificate == null) { return null; }

            RSA? key = certificate.GetRSAPublicKey(); if(key is null) { return null; }

            Aes aes = Aes.Create(); aes.KeySize = 256; aes.BlockSize = 128; aes.GenerateKey(); aes.GenerateIV(); aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.ISO10126;

            ICryptoTransform enc = aes.CreateEncryptor(); if(enc is null) { aes.Dispose(); return null; }

            Byte[] akey = key.Encrypt(aes.Key,RSAEncryptionPadding.OaepSHA512);

            Byte[] data = enc.TransformFinalBlock(input,0,input.Length);

            Byte[] res = akey.Concat(aes.IV).Concat(data).ToArray();
            
            aes.Dispose(); return res;
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="SerializeCertificate"]/*'/>*/
    public static String? SerializeCertificate(X509Certificate2? certificate)
    {
        try
        {
            if(certificate == null) { return null; }

            return certificate.Export(X509ContentType.Pkcs12).ToBase64FromByteArray();
        }
        catch ( Exception ) { if(Settings.NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToBase64FromByteArray"]/*'/>*/
    public static String ToBase64FromByteArray(this Byte[]? input) { if(input is null) { return String.Empty; } return Convert.ToBase64String(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToByteArrayFromBase64"]/*'/>*/
    public static Byte[] ToByteArrayFromBase64(this String? input) { if(input is null) { return Array.Empty<Byte>(); } return Convert.FromBase64String(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToByteArrayFromUTF16String"]/*'/>*/
    public static Byte[] ToByteArrayFromUTF16String(this String? input) { if(input is null) { return Array.Empty<Byte>(); } return Encoding.Unicode.GetBytes(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ToUTF16StringFromByteArray"]/*'/>*/
    public static String ToUTF16StringFromByteArray(this Byte[]? input) { if(input is null) { return String.Empty; } return Encoding.Unicode.GetString(input); }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="TryGetStringFromByteArray"]/*'/>*/
    public static String TryGetStringFromByteArray(this Byte[]? input)
    {
        if(input is null) { return String.Empty; }

        try { return new UTF32Encoding(false,false,true).GetString(input!); } catch ( Exception ) { }

        try { return new UnicodeEncoding(false,false,true).GetString(input!); } catch ( Exception ) { }

        try { return new UTF8Encoding(false,true).GetString(input!); } catch ( Exception ) { }

        throw new FormatException();
    }
}