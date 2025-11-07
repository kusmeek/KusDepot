namespace KusDepot.Data;

[StatePersistence(StatePersistence.None)]
internal sealed partial class Secure : Actor , ISecure
{
    private String AdminClientID {get;set;} = String.Empty;
    
    private String AdminTenantID {get;set;} = String.Empty;

    public Secure(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,SecureFail); Log.CloseAndFlush(); throw; }
    }

    public async Task<Boolean> IsAdmin(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? u = GetUPN(token); ETW.Log.IsAdminStart(u); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",u);

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(new[]{token,this.AdminTenantID,this.AdminClientID}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); ETW.Log.IsAdminError(BadArg,u); return false; }

            Log.Information(IsAdminFinish,u); SetOk(_); ETW.Log.IsAdminFinish(u); return await this.ValidateTokenVerifyRole(token,AdminRole,this.AdminTenantID,this.AdminClientID,dt,ds);
        }
        catch ( Exception _ ) { Log.Error(_,IsAdminFail,GetUPN(token)); ETW.Log.IsAdminError(_.Message,GetUPN(token)); return false; }
    }

    public async Task<Boolean> SetAdmin(String token , String tenantid , String clientid , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? u = GetUPN(token); ETW.Log.SetAdminStart(u); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",u);

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(new[]{token,tenantid,clientid}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); ETW.Log.SetAdminError(BadArg,u); return false; }

            if(await this.IsAdmin(token,dt,ds) is not true) { Log.Error(SetAdminAuthFailUser,u); SetErr(_); ETW.Log.SetAdminError(SetAdminAuthFail,u); return false; }

            this.AdminTenantID = tenantid; this.AdminClientID = clientid; Log.Information(SetAdminSuccessUser,u); SetOk(_); ETW.Log.SetAdminSuccess(u); return true;
        }
        catch ( Exception _ ) { Log.Error(_,SetAdminFail,GetUPN(token)); ETW.Log.SetAdminError(_.Message,GetUPN(token)); return false; }
    }

    public async Task<Boolean> ValidateTokenVerifyRole(String token , String role , String tenantid , String clientid , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? u = GetUPN(token); ETW.Log.ValidateVerifyStart(u,role); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",u);

            if(new String[]{token,role,tenantid,clientid}.Any(_=>String.IsNullOrEmpty(_))) { Log.Error(BadArg); SetErr(_); ETW.Log.ValidateVerifyError(BadArg,u,role); return false; }

            OpenIdConnectConfiguration cfg =
                await new ConfigurationManager<OpenIdConnectConfiguration>(OpenIDConfigURL,
                new OpenIdConnectConfigurationRetriever()).GetConfigurationAsync();

            TokenValidationParameters tvp = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(10),
                IssuerSigningKeys = cfg.SigningKeys,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidAudience = clientid,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = String.Concat("https://login.microsoftonline.com/",tenantid,"/v2.0")
            };

            TokenValidationResult tvr = await new JsonWebTokenHandler().ValidateTokenAsync(token,tvp);

            if(tvr.IsValid is false) { Log.Error(tvr.Exception,ValidateVerifyFail,u,role); SetErr(_); ETW.Log.ValidateVerifyError(tvr.Exception.Message,u,role); return false; }

            Log.Information(ValidateVerifyFinish,u,role); SetOk(_); ETW.Log.ValidateVerifyFinish(u,role);

            return ((JsonWebToken)tvr.SecurityToken).Claims.Any(_=>String.Equals(_.Type,"roles",StringComparison.Ordinal) && String.Equals(_.Value,role,StringComparison.Ordinal));
        }
        catch ( Exception _ ) { Log.Error(_,ValidateVerifyFail,GetUPN(token),role); ETW.Log.ValidateVerifyError(_.Message,GetUPN(token),role); return false; }
    }

    protected override async Task<Boolean> OnActivateAsync()
    {
        try
        {
            ETW.Log.OnActivateStart(); using DiagnosticActivity? __ = StartDiagnostic(); StorageSilo? _ = StorageSilo.FromFile(AdminFilePath);

            if(_ is null) { Log.Error(AdminLoadFail); SetErr(__); ETW.Log.OnActivateError(AdminLoadFail); return false; }

            this.AdminClientID = _.AppClientID; this.AdminTenantID = _.TenantID;

            Log.Information(Activated); SetOk(__); ETW.Log.OnActivateSuccess(); await Task.CompletedTask; return true;
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); ETW.Log.OnActivateError(_.Message); return false; }
    }

    protected override async Task<Boolean> OnDeactivateAsync() { ETW.Log.OnDeactivateStart(); ShutdownTelemetry(); Log.Information(Deactivated); await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess(); return true; }
}