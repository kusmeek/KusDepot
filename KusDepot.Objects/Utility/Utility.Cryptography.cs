namespace KusDepot;

/**<include file='Utility.xml' path='Utility/class[@name="Utility"]/main/*'/>*/
public static partial class Utility
{
    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CreateCertificate"]/*'/>*/
    public static X509Certificate2? CreateCertificate(ICommon? obj , String? subject)
    {
        return CreateCertificate(obj?.GetID(),subject,4096,10,null);
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CreateCertificate2"]/*'/>*/
    public static X509Certificate2? CreateCertificate(ICommon? obj , String? subject , Int32 rsakeysize = 4096 , Int32 yearsvalid = 10 , HashAlgorithmName? hashname = null)
    {
        try
        {
            return CreateCertificate(obj?.GetID(),subject,rsakeysize,yearsvalid,hashname);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CreateCertificateFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="CreateCertificateID"]/*'/>*/
    public static X509Certificate2? CreateCertificate(Guid? id , String? subject , Int32 rsakeysize = 4096 , Int32 yearsvalid = 10 , HashAlgorithmName? hashname = null)
    {
        try
        {
            if(id is null || Equals(Guid.Empty,id) || String.IsNullOrWhiteSpace(subject)) { return null; } if(yearsvalid <= 0) { return null; }

            if(rsakeysize < 2048 || rsakeysize > 16384 || (rsakeysize % 1024) != 0) { return null; } subject = subject.Trim();

            HashAlgorithmName a = hashname ?? (rsakeysize >= 4096 ? HashAlgorithmName.SHA512 : HashAlgorithmName.SHA256);

            using RSA k = RSA.Create(rsakeysize); String n = $"CN={id}-{subject}";

            CertificateRequest r = new(n,k,a,RSASignaturePadding.Pss); DateTimeOffset t = DateTimeOffset.UtcNow;

            return r.CreateSelfSigned(t,t.AddYears(yearsvalid));
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CreateCertificateFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Decrypt"]/*'/>*/
    public static Byte[]? Decrypt(this Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null)
    {
        try { return CryptoCore.Decrypt(input,certificate,aad); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecryptStream"]/*'/>*/
    public static Boolean Decrypt(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null)
    {
        try { return CryptoCore.Decrypt(input,output,certificate,aad); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecryptSpan"]/*'/>*/
    public static Boolean Decrypt(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default)
    {
        byteswritten = 0; try { return CryptoCore.Decrypt(input,output,certificate,out byteswritten,aad); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptSpanFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecryptAsync"]/*'/>*/
    public static Task<Byte[]?> DecryptAsync(this Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null , CancellationToken cancel = default)
    {
        try { return CryptoCore.DecryptAsync(input,certificate,aad,cancel); }

        catch ( OperationCanceledException ) { return Task.FromResult<Byte[]?>(null); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayAsyncFail); if(NoExceptions) { return Task.FromResult<Byte[]?>(null); } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DecryptStreamAsync"]/*'/>*/
    public static Task<Boolean> DecryptAsync(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null , CancellationToken cancel = default)
    {
        try { return CryptoCore.DecryptAsync(input,output,certificate,aad,cancel); }

        catch ( OperationCanceledException ) { return Task.FromResult(false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptAsyncFail); if(NoExceptions) { return Task.FromResult(false); } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="DeserializeCertificate"]/*'/>*/
    public static X509Certificate2? DeserializeCertificate(Byte[]? certificate)
    {
        try
        {
            if(certificate is null || Array.Empty<Byte>().SequenceEqual(certificate)) { return null; }

            #if NET9_0_OR_GREATER

            return X509CertificateLoader.LoadPkcs12(certificate,null,X509KeyStorageFlags.EphemeralKeySet|X509KeyStorageFlags.Exportable);

            #else

            return new X509Certificate2(certificate.ToByteArrayFromBase64(),(String?)null,X509KeyStorageFlags.EphemeralKeySet|X509KeyStorageFlags.Exportable);

            #endif
        }
        catch ( CryptographicException _ ) { KusDepotLog.Verbose(_,DeserializeCertificateFail); return null; }

        catch( Exception _ ) { KusDepotLog.Error(_,DeserializeCertificateFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="Encrypt"]/*'/>*/
    public static Byte[]? Encrypt(this Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        try { return CryptoCore.Encrypt(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower); }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="EncryptStream"]/*'/>*/
    public static Boolean Encrypt(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        try { return CryptoCore.Encrypt(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower); }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="EncryptSpan"]/*'/>*/
    public static Boolean Encrypt(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = true)
    {
        byteswritten = 0; try { return CryptoCore.Encrypt(input,output,certificate,out byteswritten,aad,includeaadhash); }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptSpanFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="EncryptAsync"]/*'/>*/
    public static Task<Byte[]?> EncryptAsync(this Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        try { return CryptoCore.EncryptAsync(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel); }

        catch (OperationCanceledException) { return Task.FromResult<Byte[]?>(null); }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayAsyncFail); if(NoExceptions) { return Task.FromResult<Byte[]?>(null); } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="EncryptStreamAsync"]/*'/>*/
    public static Task<Boolean> EncryptAsync(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        try { return CryptoCore.EncryptAsync(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel); }

        catch (OperationCanceledException) { return Task.FromResult(false); }

        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptAsyncFail); if(NoExceptions) { return Task.FromResult(false); } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="SerializeCertificate"]/*'/>*/
    public static Byte[]? SerializeCertificate(X509Certificate2? certificate)
    {
        try
        {
            return certificate?.Export(X509ContentType.Pkcs12);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SerializeCertificateFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="SignHash"]/*'/>*/
    public static Byte[]? SignHash(X509Certificate2? certificate , Byte[]? hash)
    {
        try
        {
            if(certificate is null || hash is null || Array.Empty<Byte>().SequenceEqual(hash)) { return null; }

            using var k = certificate.GetRSAPrivateKey(); if(k is null) { return null; }

            return k.SignHash(hash,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SignHashFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="SignHashKey"]/*'/>*/
    public static Byte[]? SignHash(ManagementKey? managementkey , Byte[]? hash) => SignHash(DeserializeCertificate(managementkey!.Key),hash);

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="VerifyHash"]/*'/>*/
    public static Boolean VerifyHash(X509Certificate2? certificate , Byte[]? hash , Byte[]? signature)
    {
        try
        {
            if(certificate is null || hash is null || signature is null || Array.Empty<Byte>().SequenceEqual(hash) ||
                Array.Empty<Byte>().SequenceEqual(signature)) { return false; }

            using var k = certificate.GetRSAPublicKey(); if(k is null) { return false; }

            return k.VerifyHash(hash,signature,HashAlgorithmName.SHA512,RSASignaturePadding.Pss);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,VerifyHashFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="VerifyHashKey"]/*'/>*/
    public static Boolean VerifyHash(ManagementKey? managementkey , Byte[]? hash , Byte[]? signature) => VerifyHash(DeserializeCertificate(managementkey!.Key),hash,signature);

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ZeroMemoryString"]/*'/>*/
    public static Boolean ZeroMemory(String? input)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return true; }

            unsafe
            {
                fixed(Char* c = input)
                {
                    CryptographicOperations.ZeroMemory(new Span<Byte>(c,input.Length * 2));
                }
            }

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroMemoryStringFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ZeroMemoryStream"]/*'/>*/
    public static Boolean ZeroMemory(Stream? stream)
    {
        try
        {
            if(stream is null) { return true; }

            if(!stream.CanWrite || !stream.CanSeek) { return false; }

            Int64 originalposition = stream.Position; stream.Seek(0,SeekOrigin.Begin);

            Byte[] buffer = ArrayPool<Byte>.Shared.Rent(ZeroMemoryBufferSize);

            try
            {
                CryptographicOperations.ZeroMemory(buffer);

                Int64 remaining = stream.Length;

                while(remaining > 0)
                {
                    Int32 count = (Int32)Math.Min(remaining,ZeroMemoryBufferSize);

                    stream.Write(buffer,0,count);

                    remaining -= count;
                }
            }
            finally { ArrayPool<Byte>.Shared.Return(buffer); }

            stream.Seek(originalposition,SeekOrigin.Begin);

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroMemoryStreamFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Utility.xml' path='Utility/class[@name="Utility"]/method[@name="ZeroMemoryStreamAsync"]/*'/>*/
    public static async Task<Boolean> ZeroMemoryAsync(Stream? stream , CancellationToken cancel = default)
    {
        try
        {
            if(stream is null) { return true; }

            cancel.ThrowIfCancellationRequested();

            if(!stream.CanWrite || !stream.CanSeek) { return false; }

            Int64 originalposition = stream.Position; stream.Seek(0,SeekOrigin.Begin);

            Byte[] buffer = ArrayPool<Byte>.Shared.Rent(ZeroMemoryBufferSize);

            try
            {
                CryptographicOperations.ZeroMemory(buffer);

                Int64 remaining = stream.Length;

                while(remaining > 0)
                {
                    cancel.ThrowIfCancellationRequested();

                    Int32 count = (Int32)Math.Min(remaining,ZeroMemoryBufferSize);

                    await stream.WriteAsync(new ReadOnlyMemory<Byte>(buffer,0,count),cancel).ConfigureAwait(false);

                    remaining -= count;
                }
            }
            finally { ArrayPool<Byte>.Shared.Return(buffer); }

            stream.Seek(originalposition,SeekOrigin.Begin);

            return true;
        }
        catch ( OperationCanceledException ) { return false; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ZeroMemoryStreamAsyncFail); if(NoExceptions) { return false; } throw; }
    }
}