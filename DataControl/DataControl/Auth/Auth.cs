namespace KusDepot.Data;

public sealed partial class DataControl
{
    public static void ConfigureAuth(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication().AddCertificate(o =>
        {
            o.Events = new CertificateAuthenticationEvents()
            {
                OnCertificateValidated = (c) =>
                {
                    foreach(X509Extension _ in c.ClientCertificate.Extensions)
                    {
                        if(Equals(_.Oid?.Value,X509PolicyOid))
                        {
                            c.Principal!.AddIdentity(new(new Claim[]{new(X509Claim,"true")},c.Scheme.Name));

                            c.Success(); return Task.CompletedTask;
                        }
                    }

                    c.Fail(X509AuthFail); return Task.CompletedTask;
                },

                OnAuthenticationFailed = (c) =>
                {
                    c.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask;
                }
            };

            o.RevocationMode = X509RevocationMode.NoCheck;

        });

        builder.Services.AddAuthorizationBuilder().AddPolicy(X509Policy,p => { p.RequireClaim(X509Claim,"true"); });
    }

    private static void SetupAuth(WebApplication server)
    {
        server.UseAuthentication(); server.UseAuthorization();
    }
}