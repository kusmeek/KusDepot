namespace KusDepot.Exams.Cryptography;

public static class EncryptionEnvelopeFixtureGenerator
{
    private const Int32 ChunkPower = 14;

    private static String FixtureDir
    {
        get
        {
            String? path = Environment.GetEnvironmentVariable("KusDepotSolution");

            if(String.IsNullOrWhiteSpace(path)) { throw new InvalidOperationException("KusDepotSolution environment variable is required."); }

            String root = Directory.Exists(path) ? path : Path.GetDirectoryName(path) ?? throw new InvalidOperationException("KusDepotSolution must reference a valid directory or solution file.");

            return Path.Combine(root,"KusDepot.Exam","CryptographyExams","EncryptionEnvelope","V1","Fixtures");
        }
    }

    public static void GenerateFixtures()
    {
        Directory.CreateDirectory(FixtureDir);

        using var cert = CreateCertificate(Guid.NewGuid(),"EncryptionEnvelopeV1.Compat")!; if(cert is null) { throw new InvalidOperationException("Failed to create compatibility certificate."); }

        Byte[] pfxBytes = cert.Export(X509ContentType.Pfx,"compat-test"); File.WriteAllBytes(Path.Combine(FixtureDir,"compat.pfx"),pfxBytes);

        GenerateVector(cert,"empty",Array.Empty<Byte>(),null,true,true);
        GenerateVector(cert,"small",Encoding.UTF8.GetBytes("hello world"),null,true,true);

        Byte[] multiChunk = new Byte[1 << (ChunkPower + 1)]; RandomNumberGenerator.Fill(multiChunk);
        GenerateVector(cert,"multichunk",multiChunk,null,true,true);

        GenerateVector(cert,"nolength",Encoding.UTF8.GetBytes("no-length-test"),null,true,false);

        Byte[] aad = Encoding.UTF8.GetBytes("test-aad-binding");
        GenerateVector(cert,"aad",Encoding.UTF8.GetBytes("aad-bound-payload"),aad,true,true);
    }

    private static void GenerateVector(X509Certificate2 cert , String name , Byte[] plain , Byte[]? aad , Boolean includeaadhash , Boolean includeoriginallength)
    {
        File.WriteAllBytes(Path.Combine(FixtureDir,$"{name}.plain.bin"),plain);

        Byte[]? env = Utility.Encrypt(plain,cert,aad,includeaadhash,includeoriginallength,ChunkPower); if(env is null) { throw new InvalidOperationException($"Encryption failed for vector: {name}"); }

        File.WriteAllBytes(Path.Combine(FixtureDir,$"{name}.env.bin"),env);

        if(aad is not null)
        {
            File.WriteAllBytes(Path.Combine(FixtureDir,$"{name}.aad.bin"),aad);
        }

        Byte[]? dec = Utility.Decrypt(env,cert,aad);
        if(dec is null) { throw new InvalidOperationException($"Decryption verification failed for vector: {name}"); }
        if(dec.SequenceEqual(plain) is false) { throw new InvalidOperationException($"Round-trip mismatch for vector: {name}"); }
    }
}

/*
[TestFixture]
public class EncryptionEnvelopeV1FixtureGeneratorRunner
{
    [Test]
    public void GenerateFixtures()
    {
        EncryptionEnvelopeFixtureGenerator.GenerateFixtures();
    }
}
*/