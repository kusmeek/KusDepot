namespace KusDepot.Exams.Cryptography;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public sealed class DataItemSecurityEnvelopeV1Exam
{
    /// <summary>
    /// Verifies that the V1 array encrypt and decrypt path round-trips a small payload with the expected root context.
    /// </summary>
    [Test]
    public async Task V1_EncryptArrayAsync_DecryptArrayAsync_Roundtrip_Succeeds()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.Array.Roundtrip");
        Byte[] plaintext = Encoding.UTF8.GetBytes("DataItemSecurityEnvelope V1 roundtrip payload.");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(envelope!,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(envelope, Is.Not.Null.And.Not.Empty);
        Assert.That(decrypted, Is.EqualTo(plaintext));
    }

    /// <summary>
    /// Verifies that the V1 array encrypt and decrypt path round-trips an empty payload.
    /// </summary>
    [Test]
    public async Task V1_EncryptArrayAsync_DecryptArrayAsync_EmptyPayload_Succeeds()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.Array.Empty");
        Byte[] plaintext = Array.Empty<Byte>();
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(envelope!,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(envelope, Is.Not.Null.And.Not.Empty);
        Assert.That(decrypted, Is.Not.Null);
        Assert.That(decrypted, Is.Empty);
    }

    /// <summary>
    /// Verifies that the V1 stream encrypt and decrypt path round-trips a multi-chunk payload.
    /// </summary>
    [Test]
    public async Task V1_EncryptStreamAsync_DecryptStreamAsync_MultiChunk_Succeeds()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.Stream.MultiChunk");
        Byte[] plaintext = Enumerable.Range(0,300_000).Select(_ => (Byte)(_ % 251)).ToArray();
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        await using MemoryStream input = new(plaintext,false);
        await using MemoryStream encrypted = new();
        Boolean encryptedOk = await DataItemSecurityEnvelopeV1TestHelpers.EncryptStreamAsync(input,encrypted,[issuer.Recipient],issuer.Object,rootContext,12).ConfigureAwait(false);

        encrypted.Position = 0;
        await using MemoryStream output = new();
        Boolean decryptedOk = await DataItemSecurityEnvelopeV1TestHelpers.DecryptStreamAsync(encrypted,output,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(encryptedOk, Is.True);
        Assert.That(decryptedOk, Is.True);
        Assert.That(output.ToArray(), Is.EqualTo(plaintext));
    }

    /// <summary>
    /// Verifies that the V1 encrypted-capacity estimate matches the actual serialized envelope length.
    /// </summary>
    [Test]
    public async Task V1_GetEncryptedCapacity_MatchesSerializedLength()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.Capacity");
        Byte[] plaintext = Encoding.UTF8.GetBytes(new String('x',4096));
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);
        Int32? capacity = DataItemSecurityEnvelopeV1TestHelpers.GetEncryptedCapacity([issuer.Recipient],plaintext.Length,12);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext,12).ConfigureAwait(false);

        Assert.That(capacity, Is.Not.Null);
        Assert.That(envelope, Is.Not.Null);
        Assert.That(envelope!.Length, Is.EqualTo(capacity!.Value));
    }

    /// <summary>
    /// Verifies that V1 decryption fails when the caller supplies the wrong canonical root context.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_WrongRootContext_Fails()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.WrongRoot");
        Byte[] plaintext = Encoding.UTF8.GetBytes("wrong-root-context");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);
        Byte[] wrongRootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(Guid.NewGuid());

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(envelope!,issuer.Certificate,wrongRootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.Null);
    }

    /// <summary>
    /// Verifies that V1 decryption fails when the persisted root-context hash is tampered.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_TamperedRootContextHash_Fails()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.TamperRootHash");
        Byte[] plaintext = Encoding.UTF8.GetBytes("tamper-root-hash");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[] tampered = DataItemSecurityEnvelopeV1TestHelpers.TamperRootContextHash(envelope!);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(tampered,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.Null);
    }

    /// <summary>
    /// Verifies that V1 decryption fails when the persisted authenticated metadata hash is tampered.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_TamperedEnvelopeMetadataHash_Fails()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.TamperMetadataHash");
        Byte[] plaintext = Encoding.UTF8.GetBytes("tamper-metadata-hash");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[] tampered = DataItemSecurityEnvelopeV1TestHelpers.TamperEnvelopeMetadataHash(envelope!);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(tampered,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.Null);
    }

    /// <summary>
    /// Verifies that V1 decryption fails when the serialized recipient table is tampered.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_TamperedRecipientTable_Fails()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.TamperRecipientTable");
        Byte[] plaintext = Encoding.UTF8.GetBytes("tamper-recipient-table");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[] tampered = DataItemSecurityEnvelopeV1TestHelpers.TamperRecipientTable(envelope!);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(tampered,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.Null);
    }

    /// <summary>
    /// Verifies that V1 decryption fails when ciphertext content is tampered.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_TamperedCiphertext_Fails()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.TamperCiphertext");
        Byte[] plaintext = Enumerable.Range(0,4096).Select(_ => (Byte)(_ % 239)).ToArray();
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext,12).ConfigureAwait(false);
        Byte[] tampered = DataItemSecurityEnvelopeV1TestHelpers.TamperCiphertext(envelope!);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(tampered,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.Null);
    }

    /// <summary>
    /// Verifies that V1 decryption fails when an unrelated private certificate is supplied.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_WrongCertificate_Fails()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.WrongCert.Issuer");
        using var wrong = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.WrongCert.Other");
        Byte[] plaintext = Encoding.UTF8.GetBytes("wrong-certificate");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(envelope!,wrong.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.Null);
    }

    /// <summary>
    /// Verifies that V1 envelopes can be decrypted by a secondary configured recipient.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_MultiRecipient_SecondaryRecipient_Succeeds()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.MultiRecipient.Issuer");
        using var secondary = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.MultiRecipient.Secondary");
        Byte[] plaintext = Encoding.UTF8.GetBytes("multi-recipient");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient,secondary.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(envelope!,secondary.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.EqualTo(plaintext));
    }

    /// <summary>
    /// Verifies that the V1 dispatcher rejects an unsupported version byte.
    /// </summary>
    [Test]
    public async Task V1_DecryptArrayAsync_UnsupportedVersion_Fails()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("Envelope.UnsupportedVersion");
        Byte[] plaintext = Encoding.UTF8.GetBytes("unsupported-version");
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Byte[]? envelope = await DataItemSecurityEnvelopeV1TestHelpers.EncryptArrayAsync(plaintext,[issuer.Recipient],issuer.Object,rootContext).ConfigureAwait(false);
        Byte[] tampered = DataItemSecurityEnvelopeV1TestHelpers.SetVersion(envelope!,0x7F);
        Byte[]? decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptArrayAsync(tampered,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.Null);
    }
}
