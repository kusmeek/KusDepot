namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="AddMembership"]/*'/>*/
    private Boolean AddMembership(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(AccessKeys!,SyncTime)) { throw SyncException; }

            if(AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? s)) { s.Add(token); return true; }

            return AccessKeys.TryAdd(subject,new HashSet<AccessKeyToken>{token});
        }
        catch ( Exception _ ) { Logger?.Error(_,AddMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys!)) { Exit(AccessKeys!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IsActiveMember"]/*'/>*/
    private Boolean IsActiveMember(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(AccessKeys!,SyncTime)) { throw SyncException; }

            return AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? set) && set.Contains(token);
        }
        catch { return false; }

        finally { if(IsEntered(AccessKeys!)) { Exit(AccessKeys!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="RemoveMembership"]/*'/>*/
    private Boolean RemoveMembership(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(AccessKeys!,SyncTime)) { throw SyncException; }

            if(!AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? s)) { return false; }

            if(s.Remove(token)) { if(s.Count == 0) { AccessKeys.Remove(subject); } return true; }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys!)) { Exit(AccessKeys!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="SetMembershipMetadata"]/*'/>*/
    private Boolean SetMembershipMetadata(MembershipToken membership)
    {
        try
        {
            if(!TryEnter(MembershipsByToken!,SyncTime)) { throw SyncException; }

            MembershipsByToken[membership.Token] = membership; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetMembershipMetadataFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(MembershipsByToken!)) { Exit(MembershipsByToken!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="RemoveMembershipMetadata"]/*'/>*/
    private Boolean RemoveMembershipMetadata(AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(MembershipsByToken!,SyncTime)) { throw SyncException; }

            return MembershipsByToken.Remove(token);
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(MembershipsByToken!)) { Exit(MembershipsByToken!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetMembershipMetadata"]/*'/>*/
    private Boolean TryGetMembershipMetadata(AccessKeyToken token , out MembershipToken membership)
    {
        membership = default;

        try
        {
            if(!TryEnter(MembershipsByToken!,SyncTime)) { throw SyncException; }

            return MembershipsByToken.TryGetValue(token,out membership);
        }
        catch { membership = default; return false; }

        finally { if(IsEntered(MembershipsByToken!)) { Exit(MembershipsByToken!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GetMembershipMetadataSnapshot"]/*'/>*/
    private ImmutableArray<MembershipToken> GetMembershipMetadataSnapshot()
    {
        try
        {
            if(!TryEnter(MembershipsByToken!,SyncTime)) { throw SyncException; }

            return MembershipsByToken.Values.ToImmutableArray();
        }
        catch { return []; }

        finally { if(IsEntered(MembershipsByToken!)) { Exit(MembershipsByToken!); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="MatchesAccessManagerQuery"]/*'/>*/
    private static Boolean MatchesAccessManagerQuery(AccessKeyClaims claims , AccessManagerQuery? query)
    {
        try
        {
            if(query is null) { return true; }

            if(!query.IncludeExpired && claims.Expired) { return false; }

            if(query.Subjects is ImmutableArray<String> subjects && !subjects.IsDefaultOrEmpty && !subjects.Contains(claims.Subject,StringComparer.Ordinal))
            {
                return false;
            }

            if(query.Audiences is ImmutableArray<String> audiences && !audiences.IsDefaultOrEmpty && !claims.Audiences.Any(_ => audiences.Contains(_,StringComparer.Ordinal)))
            {
                return false;
            }

            if(query.Scopes is ImmutableArray<String> scopes && !scopes.IsDefaultOrEmpty && !claims.Scopes.Any(_ => scopes.Contains(_,StringComparer.Ordinal)))
            {
                return false;
            }

            if(query.ProtectedOperations is ImmutableArray<Int32> operations && !operations.IsDefaultOrEmpty && operations.Any(_ => !claims.Allows(_)))
            {
                return false;
            }

            return true;
        }
        catch { return false; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="MatchesMembershipCatalogIdentity"]/*'/>*/
    private Boolean MatchesMembershipCatalogIdentity(AccessManagerMembershipCatalog memberships)
    {
        try
        {
            if(memberships.ToolManifestIdentity is not ToolManifestIdentity identity) { return true; }

            if(TryGetManifestIdentity(out ToolManifestIdentity currentidentity) is false) { return false; }

            return String.Equals(identity.ToolSchemaID,currentidentity.ToolSchemaID,Ordinal)
                && String.Equals(identity.AccessKeyRealmID,currentidentity.AccessKeyRealmID,Ordinal)
                && String.Equals(identity.ManifestHash,currentidentity.ManifestHash,Ordinal);
        }
        catch { return false; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.QueryMembershipCatalog)]
    public virtual AccessManagerMembershipCatalog? QueryMembershipCatalog(AccessManagerQuery? query = null , AccessKey? key = null)
    {
        try
        {
            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return null; }

            AccessManagerMembershipCatalog catalog = new();

            if(TryGetManifestIdentity(out ToolManifestIdentity? manifestidentity) is false) { return catalog; }

            DateTimeOffset evaluatedat = DateTimeOffset.UtcNow;
            Dictionary<String,List<AccessManagerMembershipEntry>> groupedentries = new();

            foreach(MembershipToken membership in GetMembershipMetadataSnapshot())
            {
                if(!String.Equals(membership.ToolSchemaID,manifestidentity.ToolSchemaID,Ordinal) ||
                   !String.Equals(membership.AccessKeyRealmID,manifestidentity.AccessKeyRealmID,Ordinal) ||
                   !String.Equals(membership.ManifestHash,manifestidentity.ManifestHash,Ordinal))
                {
                    continue;
                }

                if(!IsActiveMember(membership.Subject,membership.Token)) { continue; }

                AccessKeyClaims claims = membership.ToClaims(evaluatedat);

                if(!MatchesAccessManagerQuery(claims,query)) { continue; }

                if(!groupedentries.TryGetValue(membership.Subject,out List<AccessManagerMembershipEntry>? subjectentries))
                {
                    subjectentries = new(); groupedentries.Add(membership.Subject,subjectentries);
                }

                subjectentries.Add(AccessManagerMembershipEntry.Create(membership));
            }

            catalog.ToolManifestIdentity = manifestidentity;
            catalog.Entries = groupedentries.ToDictionary(_ => new String(_.Key),_ => _.Value.ToImmutableArray());

            return catalog;
        }
        catch ( Exception _ ) { Logger?.Error(_,QueryMembershipCatalogFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetPolicyMembership"]/*'/>*/
    private Boolean TryGetPolicyMembership(AccessKey? key , out ToolManifestIdentity manifestidentity , out MembershipToken membership , out DateTimeOffset evaluatedat , out Boolean manifestmatched , out Boolean expired , out Boolean membershipactive)
    {
        manifestidentity = null!; membership = default; evaluatedat = default; manifestmatched = false; expired = false; membershipactive = false;

        try
        {
            if(AccessKeys.Count == 0 || key is null || RealmKey is null) { return false; }

            if(TryGetManifestIdentity(out manifestidentity) is false) { return false; }

            Byte[]? secret = key.GetKey(); if(secret is null || secret.Length == 0) { return false; }

            if(MembershipCacheKey.TryCreate(secret,out MembershipCacheKey cachekey) is false) { return false; }

            if(this.MembershipCache.TryGet(cachekey,out AccessKeyToken cachedtoken))
            {
                if(TryGetMembershipMetadata(cachedtoken,out membership) is false)
                {
                    _ = this.MembershipCache.Remove(cachekey);

                    return false;
                }
            }

            else
            {
                if(AccessKeySecret.TryGetMembershipToken(secret,RealmKey,manifestidentity.ToolSchemaID,manifestidentity.AccessKeyRealmID,out MembershipToken candidate) is false) { return false; }

                if(TryGetMembershipMetadata(candidate.Token,out membership) is false) { return false; }

                _ = this.MembershipCache.TrySet(cachekey,candidate.Token);
            }

            manifestmatched = String.Equals(membership.ToolSchemaID,manifestidentity.ToolSchemaID,Ordinal) &&
                              String.Equals(membership.AccessKeyRealmID,manifestidentity.AccessKeyRealmID,Ordinal) &&
                              String.Equals(membership.ManifestHash,manifestidentity.ManifestHash,Ordinal);

            evaluatedat = DateTimeOffset.UtcNow;

            expired = membership.NotAfter is not null && evaluatedat > membership.NotAfter.Value;

            membershipactive = IsActiveMember(membership.Subject,membership.Token);

            return true;
        }
        catch
        {
            manifestidentity = null!; membership = default; evaluatedat = default; manifestmatched = false; expired = false; membershipactive = false;

            return false;
        }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetValidatedMembership"]/*'/>*/
    private Boolean TryGetValidatedMembership(AccessKey? key , out ToolManifestIdentity manifestidentity , out MembershipToken membership , out DateTimeOffset evaluatedat)
    {
        manifestidentity = null!; membership = default; evaluatedat = default;

        try
        {
            return TryGetPolicyMembership(key,out manifestidentity,out membership,out evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive)
                && manifestmatched && !expired && membershipactive;
        }
        catch { manifestidentity = null!; membership = default; evaluatedat = default; return false; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UpdateMembershipCatalog)]
    public virtual Boolean UpdateMembershipCatalog(AccessManagerMembershipCatalog? memberships , MembershipUpdateOperation operation , AccessKey? key = null)
    {
        try
        {
            if(memberships is null) { return true; }

            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return false; }

            if(!MatchesMembershipCatalogIdentity(memberships)) { return false; }

            Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>? entries = memberships.Entries;

            if(entries is null || entries.Count == 0) { return true; }

            foreach(KeyValuePair<String,ImmutableArray<AccessManagerMembershipEntry>> subjectentries in entries)
            {
                if(String.IsNullOrEmpty(subjectentries.Key) || subjectentries.Value.IsDefaultOrEmpty) { continue; }

                foreach(AccessManagerMembershipEntry entry in subjectentries.Value)
                {
                    switch(operation)
                    {
                        case MembershipUpdateOperation.Add:
                        {
                            MembershipToken membership = entry.ToMembershipToken();

                            if(SetMembershipMetadata(membership) is false) { return false; }

                            if(AddMembership(membership.Subject,membership.Token.Clone()) is false) { return false; }

                            break;
                        }

                        case MembershipUpdateOperation.Remove:
                        {
                            _ = RemoveMembership(subjectentries.Key,entry.Token);

                            _ = RemoveMembershipMetadata(entry.Token);

                            break;
                        }

                        default: { return false; }
                    }
                }
            }

            this.MembershipCache.Clear();

            return true;
        }
        catch ( Exception _ )
        {
            Logger?.Error(_,UpdateMembershipCatalogFail,this.GetType().Name,Tool?.GetID()?.ToString());

            if(MyNoExceptions) { return false; } throw;
        }
    }
}