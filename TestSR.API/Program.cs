using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Claims;
using TestSR.API;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddSignalR();
builder.Services.AddHostedService<ServerTimeNotifierService>();
builder.Services.AddTransient<ICertificateValidationService, MyCertificateValidationService>();


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});
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

app.Urls.Add("https://127.0.0.1:5555");

app.MapGet("/hello", (ClaimsPrincipal user) => Results.Ok($"Hello {user.Identity?.Name}/ auth type {user.Identity?.AuthenticationType}"));

app.MapHub<ChatHub>("notifications");

app.Run();





