using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;

namespace TestSR.API;

public class MyCertificateValidationService : ICertificateValidationService
{
    private readonly string[] validThumbprints =
    [

        "1620CCDC42EF93878099ACE5CDAE58E4B045E672"

    ];

    public bool ValidateCertificate(X509Certificate2 clientCertificate)
        => validThumbprints.Contains(clientCertificate.Thumbprint);
}

public interface ICertificateValidationService
{
    bool ValidateCertificate(X509Certificate2 clientCertificate);
}

public class ClientCertificateAuthenticationOptions : AuthenticationSchemeOptions
{
}


public class ClientCertificateAuthenticationHandler : AuthenticationHandler<ClientCertificateAuthenticationOptions>
{


    public ClientCertificateAuthenticationHandler(IOptionsMonitor<ClientCertificateAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var cert = await Request.HttpContext.Connection.GetClientCertificateAsync();
        //object cert = null;
        //if (!Request.Environment.TryGetValue("ssl.ClientCertificate", out cert) ||
        //   !(cert is X509Certificate2))
        //{
        //    s_logger.WarnFormat("Hub {0} called without certificate or cookie", hub.Context.Request.ToString());
        //    throw new Exception("not authenticated");
        //}


        //if (cert == null)
        //{
        //    return Task.FromResult<AuthenticationTicket>(null);
        //}

        //try
        //{
        //    Options.Validator.Validate(cert);
        //}
        //catch
        //{
        //    return Task.FromResult<AuthenticationTicket>(null);
        //}
        //return null;
        Claim[] claims = [ new Claim(ClaimTypes.NameIdentifier, "numeidentifier"),  new Claim(ClaimTypes.Name,"nume")];

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, Scheme.Name));

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        var r = AuthenticateResult.Success(ticket);
        return r;
    }
    //protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
    //{
    //    var cert = Context.Get<X509Certificate>("ssl.ClientCertificate");

    //    if (cert == null)
    //    {
    //        return Task.FromResult<AuthenticationTicket>(null);
    //    }

    //    try
    //    {
    //        Options.Validator.Validate(cert);
    //    }
    //    catch
    //    {
    //        return Task.FromResult<AuthenticationTicket>(null);
    //    }
    //    return null;
    //}
}