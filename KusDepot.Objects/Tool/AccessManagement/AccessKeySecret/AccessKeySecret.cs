namespace KusDepot.Security;

/**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/main/*'/>*/
public static partial class AccessKeySecret
{
    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryCreateRequest"]/*'/>*/
    public static Boolean TryCreate(AccessKeySecretCreateRequest request , Byte[]? keymaterial , out Byte[]? secret , out AccessKeyToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        secret = null; token = default;

        try
        {
            if(!TryCreateEncryptedPayload(
                keymaterial,
                request.IssuerToolID,
                request.IssuerAccessManagerID,
                request.Subject,
                request.Operations ?? Array.Empty<Int32>(),
                request.Lifetime,
                request.ToolSchemaID,
                request.AccessKeyRealmID,
                request.Audiences,
                request.Scopes,
                request.Assertions,
                request.ManifestHash,
                out Byte[]? ciphertext,
                out Byte[]? nonce,
                out Byte[]? tag,
                out token))
                { return false; }

            if(request.Signer is not IAccessKeySigner signer)
            {
                secret = BuildUnsignedEnvelope(ciphertext!,nonce!,tag!);

                return true;
            }

            AccessKeySignatureSigningContext signinginfocontext = new()
            {
                AccessKeyRealmID = request.AccessKeyRealmID ?? String.Empty,
                Logger = NullLogger.Instance,
                ManifestHash = request.ManifestHash ?? String.Empty,
                SignerAccessManagerID = request.IssuerAccessManagerID,
                SignerToolID = request.IssuerToolID,
                Subject = request.Subject,
                ToolSchemaID = request.ToolSchemaID ?? String.Empty
            };

            AccessKeySignatureInfo signinginfo = signer.GetSigningInfo(signinginfocontext);

            if(!TryValidateSigningInfo(in signinginfo)) { token = default; return false; }

            Byte[] signableenvelope = BuildSignedEnvelope(
                ciphertext!,
                nonce!,
                tag!,
                request.IssuerToolID,
                request.IssuerAccessManagerID,
                signinginfo.Algorithm,
                signinginfo.SignerKeyIDFormat,
                signinginfo.SignerKeyID.AsSpan(),
                new Byte[signinginfo.SignatureLength]);

            if(!TryReadSignedRegion(signableenvelope,out ReadOnlyMemory<Byte> signedregion))
            {
                ZeroMemory(signableenvelope); token = default; return false;
            }

            Int32 signatureoffset = signableenvelope.Length - signinginfo.SignatureLength;

            AccessKeySignatureSigningContext signingcontext = new()
            {
                AccessKeyRealmID = request.AccessKeyRealmID ?? String.Empty,
                Logger = NullLogger.Instance,
                ManifestHash = request.ManifestHash ?? String.Empty,
                SignatureInfo = signinginfo,
                SignedRegion = signedregion,
                SignerAccessManagerID = request.IssuerAccessManagerID,
                SignerToolID = request.IssuerToolID,
                Subject = request.Subject,
                ToolSchemaID = request.ToolSchemaID ?? String.Empty
            };

            if(!signer.TrySign(signingcontext,signableenvelope.AsSpan(signatureoffset,signinginfo.SignatureLength),out Int32 written) || written != signinginfo.SignatureLength)
            {
                ZeroMemory(signableenvelope); token = default; return false;
            }

            secret = signableenvelope;

            return true;
        }
        catch { secret = null; token = default; return false; }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryCreateContext"]/*'/>*/
    internal static Boolean TryCreate(AccessKeyIssuanceContext issuancecontext , Byte[]? keymaterial , IAccessKeySigner? signer , out Byte[]? secret , out AccessKeyToken token)
    {
        secret = null; token = default;

        try
        {
            ArgumentNullException.ThrowIfNull(issuancecontext);

            if(!TryCreateEncryptedPayload(
                keymaterial,
                issuancecontext.IssuerToolID,
                issuancecontext.IssuerAccessManagerID,
                issuancecontext.Subject,
                issuancecontext.IssueOptions.Operations,
                issuancecontext.IssueOptions.Lifetime,
                issuancecontext.ManifestIdentity.ToolSchemaID,
                issuancecontext.ManifestIdentity.AccessKeyRealmID,
                issuancecontext.IssueOptions.Audiences,
                issuancecontext.IssueOptions.Scopes,
                issuancecontext.Assertions,
                issuancecontext.ManifestIdentity.ManifestHash,
                out Byte[]? ciphertext,
                out Byte[]? nonce,
                out Byte[]? tag,
                out token))
                { return false; }

            if(signer is null)
            {
                secret = BuildUnsignedEnvelope(ciphertext!,nonce!,tag!);

                return true;
            }

            AccessKeySignatureSigningContext signinginfocontext = new()
            {
                AccessKeyRealmID = issuancecontext.ManifestIdentity.AccessKeyRealmID,
                Logger = issuancecontext.Logger,
                ManifestHash = issuancecontext.ManifestIdentity.ManifestHash,
                SignerAccessManagerID = issuancecontext.IssuerAccessManagerID,
                SignerToolID = issuancecontext.IssuerToolID,
                Subject = issuancecontext.Subject,
                ToolSchemaID = issuancecontext.ManifestIdentity.ToolSchemaID
            };

            AccessKeySignatureInfo signinginfo = signer.GetSigningInfo(signinginfocontext);

            if(!TryValidateSigningInfo(in signinginfo)) { token = default; return false; }

            Byte[] signableenvelope = BuildSignedEnvelope(
                ciphertext!,
                nonce!,
                tag!,
                issuancecontext.IssuerToolID,
                issuancecontext.IssuerAccessManagerID,
                signinginfo.Algorithm,
                signinginfo.SignerKeyIDFormat,
                signinginfo.SignerKeyID.AsSpan(),
                new Byte[signinginfo.SignatureLength]);

            if(!TryReadSignedRegion(signableenvelope,out ReadOnlyMemory<Byte> signedregion))
            {
                ZeroMemory(signableenvelope); token = default; return false;
            }

            Int32 signatureoffset = signableenvelope.Length - signinginfo.SignatureLength;

            AccessKeySignatureSigningContext signingcontext = new()
            {
                AccessKeyRealmID = issuancecontext.ManifestIdentity.AccessKeyRealmID,
                Logger = issuancecontext.Logger,
                ManifestHash = issuancecontext.ManifestIdentity.ManifestHash,
                SignatureInfo = signinginfo,
                SignedRegion = signedregion,
                SignerAccessManagerID = issuancecontext.IssuerAccessManagerID,
                SignerToolID = issuancecontext.IssuerToolID,
                Subject = issuancecontext.Subject,
                ToolSchemaID = issuancecontext.ManifestIdentity.ToolSchemaID
            };

            if(!signer.TrySign(signingcontext,signableenvelope.AsSpan(signatureoffset,signinginfo.SignatureLength),out Int32 written) || written != signinginfo.SignatureLength)
            {
                ZeroMemory(signableenvelope); token = default; return false;
            }

            secret = signableenvelope;

            return true;
        }
        catch { secret = null; token = default; return false; }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryCreateEncryptedPayload"]/*'/>*/
    private static Boolean TryCreateEncryptedPayload(Byte[]? keymaterial , Guid toolid , Guid accessmanagerid , String? subject , IEnumerable<Int32>? operations , TimeSpan? lifetime , String? toolschemaid , String? accesskeyrealmid , IEnumerable<String>? audiences , IEnumerable<String>? scopes , IEnumerable<AccessKeyAssertion>? assertions , String? manifesthash , out Byte[]? ciphertext , out Byte[]? nonce , out Byte[]? tag , out AccessKeyToken token)
    {
        subject ??= String.Empty; toolschemaid ??= String.Empty; accesskeyrealmid ??= String.Empty; manifesthash ??= String.Empty;
        ciphertext = null; nonce = null; tag = null; token = default;

        if(keymaterial is null || keymaterial.Length != SymmetricKeySize || toolid == Guid.Empty || accessmanagerid == Guid.Empty || operations is null) { return false; }

        String[] normalizedaudiences = NormalizeStrings(audiences);
        String[] normalizedscopes = NormalizeStrings(scopes);
        AccessKeyAssertion[] normalizedassertions = assertions?.Where(_ => _ is not null).Cast<AccessKeyAssertion>().ToArray() ?? [];
        Byte[] subjectbytes = EncodeUtf8(subject);
        Byte[] toolschemabytes = EncodeUtf8(toolschemaid);
        Byte[] accesskeyrealmbytes = EncodeUtf8(accesskeyrealmid);
        if(TryEncodeManifestHash(manifesthash,out Byte[] manifesthashbytes) is false) { return false; }
        Byte[][] audiencebytes = normalizedaudiences.Select(EncodeUtf8).ToArray();
        Byte[][] scopebytes = normalizedscopes.Select(EncodeUtf8).ToArray();
        Byte[][] assertionbytes = normalizedassertions.Select(_ => _.Serialize()).ToArray();
        Byte[] tokenbytes = new Byte[TokenIdSize];

        try
        {
            if(subjectbytes.Length > UInt16.MaxValue || toolschemabytes.Length > UInt16.MaxValue || accesskeyrealmbytes.Length > UInt16.MaxValue ||
               normalizedaudiences.Length > UInt16.MaxValue || normalizedscopes.Length > UInt16.MaxValue || normalizedassertions.Length > UInt16.MaxValue ||
               audiencebytes.Any(_ => _.Length > UInt16.MaxValue) || scopebytes.Any(_ => _.Length > UInt16.MaxValue) ||
               assertionbytes.Any(_ => _.Length == 0))
            {
                return false;
            }

            Int32[] ops = operations as Int32[] ?? operations.ToArray(); Int32 maxop = -1;

            foreach(Int32 op in ops)
            {
                if(op < 0) { return false; } if(op > maxop) { maxop = op; }
            }

            if(maxop >= MaxOperations) { return false; }

            UInt16 blockcount = maxop < 0 ? (UInt16)0 : (UInt16)((maxop / BitsPerBlock) + 1);

            Span<UInt128> blocks = stackalloc UInt128[blockcount]; blocks.Clear();

            foreach(Int32 op in ops)
            {
                Int32 block = op / BitsPerBlock; Int32 bit = op % BitsPerBlock;

                blocks[block] |= (UInt128.One << bit);
            }

            Int32 audienceslength = AudienceCountSize + audiencebytes.Sum(_ => AudienceLengthSize + _.Length);
            Int32 scopeslength = ScopeCountSize + scopebytes.Sum(_ => ScopeLengthSize + _.Length);
            Int64 assertionslength64 = AssertionCountSize + assertionbytes.Sum(_ => (Int64)AssertionLengthSize + _.Length);
            if(assertionslength64 > Int32.MaxValue) { return false; }
            Int32 assertionslength = (Int32)assertionslength64;
            Int64 issued = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Int64 notafter = lifetime.HasValue && lifetime.Value > TimeSpan.Zero ? issued + (Int64)lifetime.Value.TotalSeconds : 0L;
            Int64 plainlen64 = FixedPrefixBeforeSubject + subjectbytes.Length + ToolSchemaLengthSize + toolschemabytes.Length + AccessKeyRealmLengthSize + accesskeyrealmbytes.Length +
                ManifestHashSize + TokenIdSize + PermissionBlockCountSize + (blockcount * BytesPerBlock) + audienceslength + scopeslength + assertionslength;
            if(plainlen64 > Int32.MaxValue) { return false; }
            Int32 plainlen = (Int32)plainlen64;
            Byte[] plaintext = new Byte[plainlen]; Byte[] aad = BuildAssociatedData(toolschemaid,accesskeyrealmid);

            try
            {
                var writer = new BufferWriter(plaintext);

                if( !writer.TryWriteInt64BigEndian(issued) ||
                    !writer.TryWriteInt64BigEndian(notafter) ||
                    !writer.TryWriteGuid(toolid) ||
                    !writer.TryWriteGuid(accessmanagerid) ||
                    !writer.TryWriteUInt16BigEndian((UInt16)subjectbytes.Length) ||
                    !writer.TryWriteBytes(subjectbytes) ||
                    !writer.TryWriteUInt16BigEndian((UInt16)toolschemabytes.Length) ||
                    !writer.TryWriteBytes(toolschemabytes) ||
                    !writer.TryWriteUInt16BigEndian((UInt16)accesskeyrealmbytes.Length) ||
                    !writer.TryWriteBytes(accesskeyrealmbytes) ||
                    !writer.TryWriteBytes(manifesthashbytes) )
                {
                    return false;
                }

                RandomNumberGenerator.Fill(tokenbytes); token = new AccessKeyToken(tokenbytes);

                if(!writer.TryWriteBytes(tokenbytes) || !writer.TryWriteUInt16BigEndian(blockcount)) { token = default; return false; }

                foreach(UInt128 block in blocks)
                {
                    if(!writer.TryWriteUInt128BigEndian(block)) { return false; }
                }

                if(!writer.TryWriteUInt16BigEndian((UInt16)audiencebytes.Length)) { return false; }

                foreach(Byte[] audiencebytesentry in audiencebytes)
                {
                    if(!writer.TryWriteUInt16BigEndian((UInt16)audiencebytesentry.Length) || !writer.TryWriteBytes(audiencebytesentry)) { return false; }
                }

                if(!writer.TryWriteUInt16BigEndian((UInt16)scopebytes.Length)) { return false; }

                foreach(Byte[] scopebytesentry in scopebytes)
                {
                    if(!writer.TryWriteUInt16BigEndian((UInt16)scopebytesentry.Length) || !writer.TryWriteBytes(scopebytesentry)) { return false; }
                }

                if(!writer.TryWriteUInt16BigEndian((UInt16)assertionbytes.Length)) { return false; }

                foreach(Byte[] assertionbytesentry in assertionbytes)
                {
                    if(!writer.TryWriteUInt32BigEndian((UInt32)assertionbytesentry.Length) || !writer.TryWriteBytes(assertionbytesentry)) { return false; }
                }

                if(writer.Position != plainlen) { token = default; return false; }

                nonce = new Byte[NonceSize]; ciphertext = new Byte[plainlen]; tag = new Byte[TagSize];

                RandomNumberGenerator.Fill(nonce);

                using var gcm = new AesGcm(keymaterial,TagSize);

                gcm.Encrypt(nonce,plaintext,ciphertext,tag,aad);

                return true;
            }
            finally
            {
                ZeroMemory(plaintext); ZeroMemory(aad);
            }
        }
        finally
        {
            ZeroMemory(tokenbytes);
            ZeroMemory(subjectbytes); ZeroMemory(toolschemabytes); ZeroMemory(accesskeyrealmbytes); ZeroMemory(manifesthashbytes);
            foreach(Byte[] audiencebytesentry in audiencebytes) { ZeroMemory(audiencebytesentry); }
            foreach(Byte[] scopebytesentry in scopebytes) { ZeroMemory(scopebytesentry); }
            foreach(Byte[] assertionbytesentry in assertionbytes) { ZeroMemory(assertionbytesentry); }
        }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryParsePayload"]/*'/>*/
    private static Boolean TryParsePayload(ReadOnlySpan<Byte> secret , Byte[]? keymaterial , String? toolschemaid , String? accesskeyrealmid , [NotNullWhen(true)] out ParsedAccessKeySecretPayload? payload)
    {
        payload = null;

        if(keymaterial is null || keymaterial.Length != SymmetricKeySize)
        {
            return false;
        }

        if(!TryParseEnvelope(secret,out ParsedAccessKeySecretEnvelope envelope)) { return false; }

        Int32 cipherlen = envelope.CiphertextLength; if(cipherlen < MinPlaintextLength) { return false; }

        Byte[] aad = BuildAssociatedData(toolschemaid ?? String.Empty,accesskeyrealmid ?? String.Empty);
        Byte[] rented = ArrayPool<Byte>.Shared.Rent(cipherlen);
        Span<Byte> plaintext = rented.AsSpan(0,cipherlen);

        try
        {
            ReadOnlySpan<Byte> nonce = envelope.NonceMemory.Span;
            ReadOnlySpan<Byte> ciphertext = envelope.CiphertextMemory.Span;
            ReadOnlySpan<Byte> tag = envelope.TagMemory.Span;

            using var gcm = new AesGcm(keymaterial,TagSize);

            gcm.Decrypt(nonce,ciphertext,tag,plaintext,aad);

            var reader = new BufferReader(rented.AsMemory(0,cipherlen));
            if( !reader.TryReadInt64BigEndian(out Int64 issuedunix) ||
                !reader.TryReadInt64BigEndian(out Int64 notafterunix) ||
                !reader.TryReadBytes(GuidSize,out ReadOnlySpan<Byte> issuertoolidbytes) ||
                !reader.TryReadBytes(GuidSize,out ReadOnlySpan<Byte> issueraccessmanageridbytes) ||
                !reader.TryReadUInt16BigEndian(out UInt16 subjectlen) ||
                reader.Remaining < subjectlen + ToolSchemaLengthSize + AccessKeyRealmLengthSize + ManifestHashSize + TokenIdSize + PermissionBlockCountSize + AudienceCountSize + ScopeCountSize )
            {
                return false;
            }

            if(!reader.TryReadBytes(subjectlen,out ReadOnlySpan<Byte> subjectbytes) || !TryDecodeUtf8(subjectbytes,out String subject)) { return false; }

            if(!reader.TryReadUInt16BigEndian(out UInt16 toolschemalen) || reader.Remaining < toolschemalen + AccessKeyRealmLengthSize + ManifestHashSize + TokenIdSize + PermissionBlockCountSize + AudienceCountSize + ScopeCountSize) { return false; }
            if(!reader.TryReadBytes(toolschemalen,out ReadOnlySpan<Byte> toolschemabytes)) { return false; }

            if(!reader.TryReadUInt16BigEndian(out UInt16 accesskeyrealmlen) || reader.Remaining < accesskeyrealmlen + ManifestHashSize + TokenIdSize + PermissionBlockCountSize + AudienceCountSize + ScopeCountSize) { return false; }
            if(!reader.TryReadBytes(accesskeyrealmlen,out ReadOnlySpan<Byte> accesskeyrealmbytes)) { return false; }

            if(!reader.TryReadBytes(ManifestHashSize,out ReadOnlySpan<Byte> manifesthashbytes) || !reader.TryReadBytes(TokenIdSize,out ReadOnlySpan<Byte> tokenbytes)) { return false; }

            if( !reader.TryReadUInt16BigEndian(out UInt16 blockcount) ||
                blockcount > MaxPermissionBlocks ||
                reader.Remaining < (blockcount * BytesPerBlock) + AudienceCountSize + ScopeCountSize )
            {
                return false;
            }

            if(!reader.TryReadBytes(blockcount * BytesPerBlock,out ReadOnlySpan<Byte> bitmap)) { return false; }

            if(!reader.TryReadUInt16BigEndian(out UInt16 audiencecount)) { return false; }
            String[] audiences = new String[audiencecount];
            for(Int32 i = 0; i < audiencecount; i++)
            {
                if(!reader.TryReadUInt16BigEndian(out UInt16 audiencelen) || reader.Remaining < audiencelen + ScopeCountSize) { return false; }
                if(!reader.TryReadBytes(audiencelen,out ReadOnlySpan<Byte> audiencebytes) || !TryDecodeUtf8(audiencebytes,out audiences[i])) { return false; }
            }

            if(!reader.TryReadUInt16BigEndian(out UInt16 scopecount)) { return false; }
            String[] scopes = new String[scopecount];
            for(Int32 i = 0; i < scopecount; i++)
            {
                if(!reader.TryReadUInt16BigEndian(out UInt16 scopelen) || reader.Remaining < scopelen) { return false; }
                if(!reader.TryReadBytes(scopelen,out ReadOnlySpan<Byte> scopebytes) || !TryDecodeUtf8(scopebytes,out scopes[i])) { return false; }
            }

            if(!TryReadAssertionBlock(ref reader,out ImmutableArray<AccessKeyAssertion> assertions) || reader.Remaining != 0) { return false; }
            if(!TryDecodeUtf8(accesskeyrealmbytes,out String accesskeyrealmiddecoded) || !TryDecodeUtf8(toolschemabytes,out String toolschemaiddecoded)) { return false; }

            ImmutableArray<AccessKeyAssertionSummary> assertionsummaries = assertions.IsDefaultOrEmpty ? [] : assertions.Select(_ => _.ToSummary()).ToImmutableArray();

            DateTimeOffset issuedat = DateTimeOffset.FromUnixTimeSeconds(issuedunix);
            DateTimeOffset? notafter = notafterunix == 0 ? null : DateTimeOffset.FromUnixTimeSeconds(notafterunix);

            payload = new()
            {
                AccessKeyRealmID = accesskeyrealmiddecoded,
                Assertions = assertions,
                AssertionSummaries = assertionsummaries,
                Audiences = audiences.ToImmutableArray(),
                IssuedAt = issuedat,
                IssuerAccessManagerID = new Guid(issueraccessmanageridbytes),
                IssuerToolID = new Guid(issuertoolidbytes),
                ManifestHash = DecodeManifestHash(manifesthashbytes),
                NotAfter = notafter,
                Permissions = bitmap.ToArray(),
                Scopes = scopes.ToImmutableArray(),
                Subject = subject,
                Token = new AccessKeyToken(tokenbytes),
                ToolSchemaID = toolschemaiddecoded
            };

            if(!HasConsistentIssuerIdentity(in envelope,payload)) { payload = null; return false; }

            return true;
        }
        finally
        {
            ZeroMemory(plaintext); ZeroMemory(aad); ArrayPool<Byte>.Shared.Return(rented);
        }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryGetMembershipToken"]/*'/>*/
    internal static Boolean TryGetMembershipToken(ReadOnlySpan<Byte> secret , Byte[]? keymaterial , String? toolschemaid , String? accesskeyrealmid , out MembershipToken membershiptoken)
    {
        membershiptoken = default;

        try
        {
            if(!TryParsePayload(secret,keymaterial,toolschemaid,accesskeyrealmid,out ParsedAccessKeySecretPayload? payload) || payload is null) { return false; }

            membershiptoken = payload.ToMembershipToken();

            return true;
        }
        catch { membershiptoken = default; return false; }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadAssertions"]/*'/>*/
    public static Boolean TryReadAssertions(ReadOnlySpan<Byte> secret , Byte[]? keymaterial , String? toolschemaid , String? accesskeyrealmid , out ImmutableArray<AccessKeyAssertion> assertions)
    {
        assertions = [];

        try
        {
            if(!TryParsePayload(secret,keymaterial,toolschemaid,accesskeyrealmid,out ParsedAccessKeySecretPayload? payload) || payload is null) { return false; }

            assertions = payload.Assertions;

            return true;
        }
        catch { assertions = []; return false; }
    }

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/method[@name="TryReadClaims"]/*'/>*/
    public static Boolean TryReadClaims(ReadOnlySpan<Byte> secret , Byte[]? keymaterial , String? toolschemaid , String? accesskeyrealmid , out AccessKeyClaims claims)
    {
        claims = null!;

        try
        {
            if(!TryParsePayload(secret,keymaterial,toolschemaid,accesskeyrealmid,out ParsedAccessKeySecretPayload? payload) || payload is null) { return false; }

            claims = payload.ToClaims(DateTimeOffset.UtcNow);

            return true;
        }
        catch { claims = null!; return false; }
    }
}