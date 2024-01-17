using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;
using System.Security.Claims;
using TestSR.API;

var builder = WebApplication.CreateBuilder(args);

ServicePointManager.ServerCertificateValidationCallback +=
    (sender, certificate, chain, sslPolicyErrors) =>
    {
        return true;
    };


builder.Services.AddSignalR();
builder.Services.AddHostedService<ServerTimeNotifierService>();


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

//builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
//    .AddCertificate(options =>
//    {

//        options.ValidateCertificateUse = false;
//        options.AllowedCertificateTypes = CertificateTypes.All;
//        options.Events = new CertificateAuthenticationEvents
//        {

//            OnCertificateValidated = context =>
//                {
//                    var validationService = context.HttpContext.RequestServices
//                        .GetRequiredService<ICertificateValidationService>();


//                    var claims = new[]
//                    {
//                    new Claim(
//                        ClaimTypes.NameIdentifier,
//                        context.ClientCertificate.Subject,
//                        ClaimValueTypes.String, context.Options.ClaimsIssuer),
//                    new Claim(
//                        ClaimTypes.Name,
//                        context.ClientCertificate.Subject,
//                        ClaimValueTypes.String, context.Options.ClaimsIssuer)
//                    };


//                    context.Principal = new ClaimsPrincipal(
//                        new ClaimsIdentity(claims, context.Scheme.Name));
//                    context.Success();


//                    return Task.CompletedTask;
//                },
//            OnAuthenticationFailed = context =>
//            {
//                return Task.CompletedTask;
//            },
//            OnChallenge = context =>
//            {
//                var cer = context.HttpContext.Connection.ClientCertificate;
//                //Console.WriteLine(context.Request.ToString());
//                return Task.CompletedTask;

//            }


//        };
//    });

builder.Services.AddTransient<ICertificateValidationService, MyCertificateValidationService>();

builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<ClientCertificateAuthenticationOptions, ClientCertificateAuthenticationHandler>(CertificateAuthenticationDefaults.AuthenticationScheme, null);

builder.Logging.AddConsole();



builder.WebHost.UseKestrel(options =>
{

    options.ConfigureEndpointDefaults(o =>
    {
        o.UseHttps("E:\\certs\\server.pfx", "12345");
    });

    options.ConfigureHttpsDefaults(conf =>
    {

        conf.CheckCertificateRevocation = false;
        conf.ClientCertificateValidation = (a, b, d) =>
        {
            return true;
        };
        conf.ClientCertificateMode = ClientCertificateMode.RequireCertificate;

    });
});





var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

app.Urls.Add("https://127.0.0.1:5555");

app.MapGet("/hello", (ClaimsPrincipal user) => Results.Ok($"Hello {user.Identity?.Name}/ auth type {user.Identity?.AuthenticationType}"));




app.MapHub<ChatHub>("notifications");

app.Run();





