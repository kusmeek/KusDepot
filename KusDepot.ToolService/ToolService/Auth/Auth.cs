namespace KusDepot.ToolService;

public sealed partial class ToolService
{
    internal sealed class X509PolicyValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            foreach(X509Extension _ in certificate.Extensions)
            {
                if(Equals(_.Oid?.Value,X509PolicyOid))
                {
                    return;
                }
            }

            throw new SecurityTokenValidationException(X509AuthFail);
        }
    }
}