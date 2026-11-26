namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="AddMembership"]/*'/>*/
    private Boolean AddMembership(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            if(AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? s)) { s.Add(token); return true; }

            return AccessKeys.TryAdd(subject,new HashSet<AccessKeyToken>{token});
        }
        catch ( Exception _ ) { Logger?.Error(_,AddMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IsActiveMember"]/*'/>*/
    private Boolean IsActiveMember(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            return AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? set) && set.Contains(token);
        }
        catch { return false; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="RemoveMembership"]/*'/>*/
    private Boolean RemoveMembership(String subject , AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            if(!AccessKeys.TryGetValue(subject,out HashSet<AccessKeyToken>? s)) { return false; }

            if(s.Remove(token)) { if(s.Count == 0) { AccessKeys.Remove(subject); } return true; }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="SetMembershipMetadata"]/*'/>*/
    private Boolean SetMembershipMetadata(MembershipToken membership)
    {
        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            MembershipsByToken[membership.Token] = membership; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetMembershipMetadataFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="RemoveMembershipMetadata"]/*'/>*/
    private Boolean RemoveMembershipMetadata(AccessKeyToken token)
    {
        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            return MembershipsByToken.Remove(token);
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveMembershipFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetMembershipMetadata"]/*'/>*/
    private Boolean TryGetMembershipMetadata(AccessKeyToken token , out MembershipToken membership)
    {
        membership = default;

        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            return MembershipsByToken.TryGetValue(token,out membership);
        }
        catch { membership = default; return false; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GetMembershipMetadataSnapshot"]/*'/>*/
    private ImmutableArray<MembershipToken> GetMembershipMetadataSnapshot()
    {
        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            return MembershipsByToken.Values.ToImmutableArray();
        }
        catch { return []; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="HasMemberships"]/*'/>*/
    private Boolean HasMemberships()
    {
        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            return AccessKeys.Count > 0;
        }
        catch { return false; }

        finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
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
            if(memberships.ToolManifestIdentity is not ToolManifestIdentity identity) { return false; }

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

            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            try
            {
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
            finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
        }
        catch ( Exception _ ) { Logger?.Error(_,QueryMembershipCatalogFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetPolicyMembership"]/*'/>*/
    private Boolean TryGetPolicyMembership(AccessKey? key , out ToolManifestIdentity manifestidentity , out MembershipToken membership , out DateTimeOffset evaluatedat , out Boolean manifestmatched , out Boolean expired , out Boolean membershipactive , out Boolean issueraccessmanagermatched , out Boolean issuertoolmatched)
    {
        manifestidentity = null!; membership = default; evaluatedat = default; manifestmatched = false; expired = false; membershipactive = false; issueraccessmanagermatched = false; issuertoolmatched = false;

        try
        {
            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            try
            {
                if(!HasMemberships() || key is null || RealmKey is null) { return false; }

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

                issueraccessmanagermatched = ID is Guid accessmanagerid && accessmanagerid != Guid.Empty && membership.IssuerAccessManagerID == accessmanagerid;

                issuertoolmatched = Tool?.GetID() is Guid toolid && toolid != Guid.Empty && membership.IssuerToolID == toolid;

                manifestmatched = String.Equals(membership.ToolSchemaID,manifestidentity.ToolSchemaID,Ordinal) &&
                                  String.Equals(membership.AccessKeyRealmID,manifestidentity.AccessKeyRealmID,Ordinal) &&
                                  String.Equals(membership.ManifestHash,manifestidentity.ManifestHash,Ordinal);

                evaluatedat = DateTimeOffset.UtcNow;

                expired = membership.NotAfter is not null && evaluatedat > membership.NotAfter.Value;

                membershipactive = IsActiveMember(membership.Subject,membership.Token);

                return true;
            }
            finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }
        }
        catch
        {
            manifestidentity = null!; membership = default; evaluatedat = default; manifestmatched = false; expired = false; membershipactive = false;

            issueraccessmanagermatched = false; issuertoolmatched = false;

            return false;
        }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryGetValidatedMembership"]/*'/>*/
    private Boolean TryGetValidatedMembership(AccessKey? key , out ToolManifestIdentity manifestidentity , out MembershipToken membership , out DateTimeOffset evaluatedat)
    {
        manifestidentity = null!; membership = default; evaluatedat = default;

        try
        {
            return TryGetPolicyMembership(key,out manifestidentity,out membership,out evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive,out Boolean issueraccessmanagermatched,out Boolean issuertoolmatched)
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

            Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>? entries = memberships.Entries;

            if(entries is null || entries.Count == 0) { return true; }

            if(operation is not MembershipUpdateOperation.Add and not MembershipUpdateOperation.Remove) { return false; }

            if(Tool?.GetLocked() is true && AccessCheck(key) is false) { return false; }

            if(!MatchesMembershipCatalogIdentity(memberships)) { return false; }

            List<(String Subject,MembershipToken Membership)> staged = new();

            foreach(KeyValuePair<String,ImmutableArray<AccessManagerMembershipEntry>> subjectentries in entries)
            {
                if(subjectentries.Value.IsDefaultOrEmpty) { continue; }

                if(String.IsNullOrEmpty(subjectentries.Key)) { return false; }

                foreach(AccessManagerMembershipEntry entry in subjectentries.Value)
                {
                    if(entry is null || !String.Equals(subjectentries.Key,entry.Subject,Ordinal)) { return false; }

                    if(!String.Equals(entry.ToolSchemaID,memberships.ToolManifestIdentity!.ToolSchemaID,Ordinal) ||
                       !String.Equals(entry.AccessKeyRealmID,memberships.ToolManifestIdentity.AccessKeyRealmID,Ordinal) ||
                       !String.Equals(entry.ManifestHash,memberships.ToolManifestIdentity.ManifestHash,Ordinal))
                    {
                        return false;
                    }

                    staged.Add((subjectentries.Key,entry.ToMembershipToken()));
                }
            }

            if(!TryEnter(MembershipSync,SyncTime)) { throw SyncException; }

            try
            {
                Dictionary<String,HashSet<AccessKeyToken>> accesskeys = AccessKeys.ToDictionary(_ => _.Key,_ => new HashSet<AccessKeyToken>(_.Value),StringComparer.Ordinal);

                Dictionary<AccessKeyToken,MembershipToken> membershipsbytoken = MembershipsByToken.ToDictionary(_ => _.Key,_ => _.Value);

                foreach((String subject,MembershipToken membership) in staged)
                {
                    if(operation is MembershipUpdateOperation.Add)
                    {
                        membershipsbytoken[membership.Token] = membership;

                        if(!accesskeys.TryGetValue(subject,out HashSet<AccessKeyToken>? tokens))
                        {
                            tokens = new(); accesskeys.Add(subject,tokens);
                        }

                        _ = tokens.Add(membership.Token.Clone());
                    }

                    else
                    {
                        if(accesskeys.TryGetValue(subject,out HashSet<AccessKeyToken>? tokens) && tokens.Remove(membership.Token))
                        {
                            if(tokens.Count == 0) { _ = accesskeys.Remove(subject); }
                        }

                        _ = membershipsbytoken.Remove(membership.Token);
                    }
                }

                AccessKeys = accesskeys; MembershipsByToken = membershipsbytoken;

                this.MembershipCache.Clear();
            }
            finally { if(IsEntered(MembershipSync)) { Exit(MembershipSync); } }

            return true;
        }
        catch ( Exception _ )
        {
            Logger?.Error(_,UpdateMembershipCatalogFail,this.GetType().Name,Tool?.GetID()?.ToString());

            if(MyNoExceptions) { return false; } throw;
        }
    }
}