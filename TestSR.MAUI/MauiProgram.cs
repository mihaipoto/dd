using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using System.Net;
using TestSR.MAUI.Services;

namespace TestSR.MAUI;

public static class MauiProgram
{


    public static MauiApp CreateMauiApp()
    {

        ServicePointManager.ServerCertificateValidationCallback +=
    (sender, certificate, chain, sslPolicyErrors) =>
    {
        return true;
    };

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingletonWithShellRoute<MainPage, MainVM>(nameof(MainPage));
        builder.Services.AddSingleton<SrService>();
        builder.Services.AddKeyedSingleton<IMyRetryPolicy, MyRetryPolicy>(KeyedPolicies.MyPolicy);
        builder.Services.AddSingleton<IClientCertificateProvider, ClientCertificateProvider>();
        builder.Services.AddHttpClient();
        //builder.Services.AddHttpClient("namedClient").ConfigurePrimaryHttpMessageHandler((serviceProvider) =>
        //{

        //    //var certificateProvider = serviceProvider.GetService<IClientCertificateProvider>();
        //    //if (certificateProvider != null)
        //    //{
        //    //    var sslOptions = new SslClientAuthenticationOptions()
        //    //    {
        //    //        ClientCertificates = certificateProvider.GetClientCertificates(),
        //    //        RemoteCertificateValidationCallback = (a, b, n, c) =>
        //    //        {
        //    //            return true;
        //    //        }
        //    //    };
        //    //    var handler = new SocketsHttpHandler
        //    //    {
        //    //        SslOptions = sslOptions
        //    //    };
        //    //    return handler;
        //    //}
        //    //else
        //    //{
        //    //    var handler = new SocketsHttpHandler();
        //    //    return handler;
        //    //    //no cert provider
        //    //}
        //});



        return builder.Build();
    }
}
