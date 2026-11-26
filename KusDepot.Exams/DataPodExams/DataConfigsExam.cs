using DataPodServices.DataConfigs;

namespace KusDepot.DataPodExams;

[TestFixture] [NonParallelizable]
public class DataConfigsExam
{
    private IToolGenericHost? Host;
    private IDataControlDataConfigsClient? DataConfigs;

    public String? AdminUserName;
    public String? AdminUserPass;
    public String? AdminToken;
    public String? ReadUserName;
    public String? ReadUserPass;
    public String? ReadToken;
    public String? WriteUserName;
    public String? WriteUserPass;
    public String? WriteToken;

    public String? ClientID;
    public String? TenantID;
    public String? Authority;
    public String[]? Scope;


    [OneTimeSetUp]
    public async Task Calibrate()
    {
        Check.That(await this.LoadSetup()).IsTrue();

        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        tb.Builder.UseOrleansClient(builder =>
        {
            OrleansExamClient.Configure(builder);
            builder.Services.AddDataControlDataConfigsClient();
        });

        Host = await tb.BuildGenericHostAsync(); await Host.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);

        DataConfigs = Host.Services.GetRequiredService<IDataControlDataConfigsClient>();

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.AdminToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync()).AccessToken;

        this.ReadToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync()).AccessToken;

        this.WriteToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync()).AccessToken;

        Check.That(GetRoles(this.ReadToken!)).Contains("Catalog.Read");
        Check.That(GetRoles(this.WriteToken!)).Contains("Catalog.Write");
    }

    [OneTimeTearDown]
    public async Task Complete() { await Host!.DisposeAsync(); }

    [Test]
    public async Task ResolveTenantStorageForRead()
    {
        TenantStorageResolution missingToken = await this.DataConfigs!.ResolveTenantStorageAsync(String.Empty,CatalogAccess.Read,null);
        TenantStorageResolution invalidToken = await this.DataConfigs.ResolveTenantStorageAsync("not-a-token",CatalogAccess.Read,null);
        TenantStorageResolution authorized = await this.DataConfigs.ResolveTenantStorageAsync(this.ReadToken!,CatalogAccess.Read,null);

        Check.That(missingToken.Status).IsEqualTo(CatalogResolutionStatus.Forbidden);
        Check.That(invalidToken.Status).IsEqualTo(CatalogResolutionStatus.Forbidden);
        Check.That(authorized.Status).IsEqualTo(CatalogResolutionStatus.Authorized);
        Check.That(authorized.CatalogName).IsEqualTo("PublicCatalog");
        Check.That(authorized.ConnectionString).IsNotNull();
    }

    [Test]
    public async Task ResolveTenantStorageForWrite()
    {
        TenantStorageResolution missingToken = await this.DataConfigs!.ResolveTenantStorageAsync(String.Empty,CatalogAccess.Write,null);
        TenantStorageResolution invalidToken = await this.DataConfigs.ResolveTenantStorageAsync("not-a-token",CatalogAccess.Write,null);
        TenantStorageResolution readOnly = await this.DataConfigs.ResolveTenantStorageAsync(this.ReadToken!,CatalogAccess.Write,null);
        TenantStorageResolution authorized = await this.DataConfigs.ResolveTenantStorageAsync(this.WriteToken!,CatalogAccess.Write,null);

        Check.That(missingToken.Status).IsEqualTo(CatalogResolutionStatus.Forbidden);
        Check.That(invalidToken.Status).IsEqualTo(CatalogResolutionStatus.Forbidden);
        Check.That(readOnly.Status).IsEqualTo(CatalogResolutionStatus.Forbidden);
        Check.That(readOnly.ConnectionString).IsNull();
        Check.That(authorized.Status).IsEqualTo(CatalogResolutionStatus.Authorized);
        Check.That(authorized.CatalogName).IsEqualTo("PublicCatalog");
        Check.That(authorized.ConnectionString).IsNotNull();
    }

    [Test]
    public async Task CheckAdminAuthorization()
    {
        AdminCheckResult admin = await this.DataConfigs!.CheckAdminAsync(this.AdminToken!,null);
        AdminCheckResult reader = await this.DataConfigs.CheckAdminAsync(this.ReadToken!,null);
        AdminCheckResult invalid = await this.DataConfigs.CheckAdminAsync("not-a-token",null);

        Check.That(admin.Status).IsEqualTo(AdminCheckStatus.Authorized);
        Check.That(reader.Status).IsEqualTo(AdminCheckStatus.Forbidden);
        Check.That(invalid.Status).IsEqualTo(AdminCheckStatus.Forbidden);
    }

    [Test]
    public async Task GetCatalogSnapshotForAdmin()
    {
        CatalogSnapshotResult snapshot = await this.DataConfigs!.GetCatalogSnapshotAsync(this.AdminToken!,null);

        Check.That(snapshot.Catalogs.Length).IsEqualTo(1);

        Check.That(snapshot.Catalogs.Any(s=>String.Equals(s.CatalogName,"PublicCatalog",StringComparison.Ordinal)
            && String.Equals(s.TenantId,this.TenantID,StringComparison.Ordinal)
            && String.Equals(s.AppClientId,this.ClientID,StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task GetCatalogSnapshotRejectsNonAdmin()
    {
        Check.ThatAsyncCode(async () => await this.DataConfigs!.GetCatalogSnapshotAsync(this.ReadToken!,null)).Throws<UnauthorizedAccessException>();
    }

    [Test]
    public async Task GetConfiguredStorageForRecovery()
    {
        TenantStorageResolution configured = await this.DataConfigs!.GetConfiguredStorageForRecoveryAsync("PublicCatalog");
        TenantStorageResolution missing = await this.DataConfigs.GetConfiguredStorageForRecoveryAsync("MissingCatalog");

        Check.That(configured.Status).IsEqualTo(CatalogResolutionStatus.Authorized);
        Check.That(configured.CatalogName).IsEqualTo("PublicCatalog");
        Check.That(configured.ConnectionString).IsNotNull();
        Check.That(missing.Status).IsEqualTo(CatalogResolutionStatus.NotConfigured);
        Check.That(missing.ConnectionString).IsNull();
    }


    private async Task<Boolean> LoadSetup()
    {
        try
        {
            EntraSetupRecord setup = await EntraSetupProvider.LoadAsync().ConfigureAwait(false);

            this.Authority = setup.Authority;
            this.TenantID = setup.TenantID;
            this.ClientID = setup.ClientID;
            this.Scope = [setup.Scope];
            this.AdminUserName = setup.AdminUserName;
            this.AdminUserPass = setup.AdminUserPass;
            this.ReadUserName = setup.ReadUserName;
            this.ReadUserPass = setup.ReadUserPass;
            this.WriteUserName = setup.WriteUserName;
            this.WriteUserPass = setup.WriteUserPass;

            return true;
        }
        catch (Exception) { return false; }
    }

    private static IEnumerable<String> GetRoles(String token)
    {
        String[] segments = token.Split('.');

        if(segments.Length < 2) { return Array.Empty<String>(); }

        Int32 padding = segments[1].Length % 4; String payload = segments[1].Replace('-','+').Replace('_','/');

        if(padding > 0) { payload = payload.PadRight(payload.Length + 4 - padding,'='); }

        try
        {
            using JsonDocument document = JsonDocument.Parse(Convert.FromBase64String(payload));

            if(document.RootElement.TryGetProperty("roles",out JsonElement roles) is false || roles.ValueKind is not JsonValueKind.Array) { return Array.Empty<String>(); }

            return roles.EnumerateArray().Where(static role => role.ValueKind is JsonValueKind.String).Select(static role => role.GetString()!).ToArray();
        }
        catch { return Array.Empty<String>(); }
    }
}