namespace KusDepot;

/**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/main/*'/>*/
public sealed partial class CommonSecret
{
    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/property[@name="Version"]/*'/>*/
    public Byte Version => version; private readonly Byte version; private readonly Byte[] secret;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/constructor[@name="Constructor"]/*'/>*/
    private CommonSecret(Byte[] secret , Byte version) { this.secret = secret; this.version = version; }

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/method[@name="CreateV1"]/*'/>*/
    public static CommonSecret? CreateV1(X509Certificate2? certificate , Guid? objectid)
    {
        if(certificate is null || objectid is null || Equals(Guid.Empty,objectid)) { return null; }

        ReadOnlySpan<Byte> s = certificate.SerialNumberBytes.Span;

        Int32 sl = s.Length; if(sl == 0 || sl > MaxSerialNumberLength) { return null; }

        Span<Byte> salt = stackalloc Byte[SaltSize];
        Span<Byte> hash = stackalloc Byte[HashSize];
        Span<Byte> cmb = stackalloc Byte[sl + SaltSize];
        Span<Byte> aad = stackalloc Byte[sl + GuidSize];
        Span<Byte> pt = stackalloc Byte[PlaintextPrefixSize + sl + SaltSize + HashSize];
        Span<Byte> env = stackalloc Byte[VersionSize + RsaLengthSize + RsaKeyLimit + NonceSize + pt.Length + TagSize];

        try
        {
            pt[0] = V1; pt[1] = (Byte)sl;
            RandomNumberGenerator.Fill(salt);
            s.CopyTo(pt[PlaintextPrefixSize..]);
            salt.CopyTo(pt[(PlaintextPrefixSize + sl)..]);
            s.CopyTo(cmb);
            salt.CopyTo(cmb[sl..]);
            SHA512.HashData(cmb,hash);
            hash.CopyTo(pt[(PlaintextPrefixSize + sl + SaltSize)..]);
            ReadOnlySpan<Byte> id = objectid.Value.ToByteArray().AsSpan();
            s.CopyTo(aad); id.CopyTo(aad[sl..]);

            if(!Encrypt(pt,env,certificate,out Int32 w,aad,true) || w == 0) { return null; }

            return new CommonSecret(env[..w].ToArray(),V1);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,CreateV1Fail); if(NoExceptions) { return null; } throw; }

        finally { ZeroMemory(pt); ZeroMemory(salt); ZeroMemory(hash); ZeroMemory(cmb); ZeroMemory(aad); ZeroMemory(env); }
    }

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/method[@name="TryValidateV1"]/*'/>*/
    internal static Boolean TryValidateV1(ReadOnlySpan<Byte> secret , X509Certificate2? certificate , Guid? objectid)
    {
        if(secret.IsEmpty || certificate is null || objectid is null || Equals(Guid.Empty,objectid)) { return false; }

        ReadOnlySpan<Byte> s = certificate.SerialNumberBytes.Span;

        if(s.Length == 0 || s.Length > MaxSerialNumberLength) { return false; }

        Span<Byte> chash = stackalloc Byte[HashSize];
        Span<Byte> cmb = stackalloc Byte[s.Length + SaltSize];
        Span<Byte> aad = stackalloc Byte[s.Length + GuidSize];
        Span<Byte> pt = stackalloc Byte[PlaintextPrefixSize + MaxSerialNumberLength + SaltSize + HashSize];

        try
        {
            ReadOnlySpan<Byte> id = objectid.Value.ToByteArray().AsSpan();
            s.CopyTo(aad); id.CopyTo(aad[s.Length..]);

            if(!Decrypt(secret,pt,certificate,out Int32 w,aad) || w < PlaintextPrefixSize)        { return false; }

            if(pt[0] != V1)                                                                       { return false; }
            Int32 sLen = pt[1]; if(sLen != s.Length || sLen <= 0 || sLen > MaxSerialNumberLength) { return false; }
            if(w != PlaintextPrefixSize + sLen + SaltSize + HashSize)                             { return false; }
            if(!pt.Slice(PlaintextPrefixSize,sLen).SequenceEqual(s))                              { return false; }

            ReadOnlySpan<Byte> salt = pt.Slice(PlaintextPrefixSize + sLen,SaltSize);
            ReadOnlySpan<Byte> shash = pt.Slice(PlaintextPrefixSize + sLen + SaltSize,HashSize);
            s.CopyTo(cmb);
            salt.CopyTo(cmb[s.Length..]);
            SHA512.HashData(cmb,chash);

            return FixedTimeEquals(shash,chash);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,TryValidateV1Fail); if(NoExceptions) { return false; } throw; }

        finally { ZeroMemory(pt); ZeroMemory(aad); ZeroMemory(cmb); ZeroMemory(chash); }
    }

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/method[@name="TryValidate"]/*'/>*/
    public static Boolean TryValidate(ReadOnlySpan<Byte> secret , X509Certificate2? certificate , Guid? objectid)
    {
        return secret.IsEmpty ? false : TryValidateV1(secret,certificate,objectid);
    }

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/method[@name="GetBytes"]/*'/>*/
    public Byte[] GetBytes() => secret.ToArray();
}