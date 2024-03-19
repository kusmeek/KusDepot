namespace KusDepot.FabricExams;

[TestFixture] [Parallelizable]
public class SecureExam
{
    public ISecure? Proxy;
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
    public void Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue();

        this.Proxy = ActorProxy.Create<ISecure>(new ActorId(Guid.NewGuid()),ServiceLocators.SecureService);

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.AdminToken = _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync().Result.AccessToken;

        this.ReadToken = _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync().Result.AccessToken;

        this.WriteToken = _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync().Result.AccessToken;
    }

    [Test] [Order(2)]
    public void IsAdmin()
    {
        Check.That(this.Proxy!.IsAdmin(this.AdminToken!,null,null).Result).IsTrue();

        Check.That(this.Proxy!.IsAdmin(this.ReadToken!,null,null).Result).IsFalse();
    }

    [Test] [Order(3)]
    public void SetAdmin()
    {
        Check.That(this.Proxy!.SetAdmin(this.ReadToken!,String.Empty,String.Empty,null,null).Result).IsFalse();

        Check.That(this.Proxy!.SetAdmin(this.AdminToken!,String.Empty,String.Empty,null,null).Result).IsFalse();

        Check.That(this.Proxy!.SetAdmin(this.AdminToken!,this.TenantID!,this.ClientID!,null,null).Result).IsTrue();
    }

    [Test] [Order(1)]
    public void ValidateTokenVerifyRole()
    {
        Check.That(this.Proxy!.ValidateTokenVerifyRole(this.ReadToken!,"Non-Existent-Role",this.TenantID!,this.ClientID!,null,null).Result).IsFalse();

        Check.That(this.Proxy!.ValidateTokenVerifyRole(this.ReadToken!,"PublicCatalog.Read",this.TenantID!,this.ClientID!,null,null).Result).IsTrue();

        Check.That(this.Proxy!.ValidateTokenVerifyRole(this.WriteToken!,"PublicCatalog.Write",this.TenantID!,this.ClientID!,null,null).Result).IsTrue();
    }

    private Boolean LoadSetup()
    {
        try
        {
            XmlDocument d = new XmlDocument(); d.Load(Environment.CurrentDirectory + @"\EntraSetup.xml"); XmlNode? n = d.SelectSingleNode("EntraSetup");

            this.Authority     = n!.SelectSingleNode("Authority")!.InnerText;
            this.TenantID      = n.SelectSingleNode("TenantID")!.InnerText;
            this.ClientID      = n.SelectSingleNode("ClientID")!.InnerText;
            this.Scope         = new List<String>(){n.SelectSingleNode("Scope")!.InnerText}.ToArray();
            this.AdminUserName = n.SelectSingleNode("AdminUserName")!.InnerText;
            this.AdminUserPass = n.SelectSingleNode("AdminUserPass")!.InnerText;
            this.ReadUserName  = n.SelectSingleNode("ReadUserName")!.InnerText;
            this.ReadUserPass  = n.SelectSingleNode("ReadUserPass")!.InnerText;
            this.WriteUserName = n.SelectSingleNode("WriteUserName")!.InnerText;
            this.WriteUserPass = n.SelectSingleNode("WriteUserPass")!.InnerText;

            return true;
        }
        catch (Exception) { return false; }
    }
}