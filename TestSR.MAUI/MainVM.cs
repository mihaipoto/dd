using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Net.Security;
using TestSR.MAUI.Services;

namespace TestSR.MAUI;

public partial class MainVM(
    SrService srService, ILogger<MainVM> logger,
    [FromKeyedServices(KeyedPolicies.MyPolicy)] IMyRetryPolicy policy,
    IClientCertificateProvider clientCertificateProvider,
    IHttpClientFactory httpClientFactory) : ObservableObject
{
    public ObservableCollection<string> Messages { get; set; } = [];

    [ObservableProperty]
    private DateTime _currentTime = DateTime.MinValue;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reason))]
    [NotifyPropertyChangedFor(nameof(TimeSinceDisconnection))]
    [NotifyPropertyChangedFor(nameof(NoOfRetryes))]
    RetryContext? _retryContext;

    public string Reason => RetryContext?.RetryReason?.Message ?? "No retry reason";
    public string TimeSinceDisconnection => RetryContext?.ElapsedTime.ToString() ?? string.Empty;
    public string NoOfRetryes => RetryContext?.PreviousRetryCount.ToString() ?? string.Empty;


    private SslClientAuthenticationOptions _sslOptions;
    private SocketsHttpHandler _socksHttpHandler;



    [RelayCommand]
    public async Task CallServer()
    {



        var httpClient = httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.GetAsync("https://localhost:5555/hello");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            //var resp = JsonDocument.Parse(
            //    await httpResponseMessage.Content.ReadAsStringAsync());
            Messages.Add("OK");
        }
        else
        {
            Messages.Add($"{httpResponseMessage.StatusCode}");
        }

    }


    [RelayCommand]
    public async Task ConnectToHub()
    {


    }



    public async void PageLoaded()
    {

        await Task.Delay(500);

        _sslOptions = new SslClientAuthenticationOptions()
        {

            ClientCertificates = clientCertificateProvider.GetClientCertificates("u1"),
            RemoteCertificateValidationCallback = (a, b, n, c) =>
            {
                return true;
            }
        };

        _socksHttpHandler = new SocketsHttpHandler
        {
            SslOptions = _sslOptions
        };


        policy.HandleReconnectionAttempt((context) => RetryContext = context);
        srService.ConfigureService((options) =>
        {
            options.Url = "https://localhost:5555/notifications";
            options.RetryPolicy = policy;
            options.ConnectionOptions = new()
            {
                HttpMessageHandlerFactory = handler => new SocketsHttpHandler()
                {
                    SslOptions = _sslOptions
                },
                //WebSocketConfiguration = sockets =>
                //{
                //    /sockets.ClientCertificates = SrService.GetClientCertificates("dd");

                //    sockets.RemoteCertificateValidationCallback = (sender, certificate, chain, policyErrors) =>
                //    {
                //        return true;
                //    };
                //},

            };

            options.Events = new SrServiceEvents()
            {
                ConnectionClosedHandler = (ex) => Messages.Add(ex?.Message ?? "Connection closed"),
                ExceptionHandler = (ex) => Messages.Add(ex?.ToString() ?? "Eroare"),
                ReceiveNotificationHandler = (message) => Messages.Add(message),
                ReceiveTimenHandler = (newTime) => CurrentTime = newTime,
                ReconnectedHandler = () => { Messages.Add("Reconnected"); RetryContext = null; }
            };
        });

        await srService.ConnectToHub();



    }



    //private async Task<(byte[]? PinBytes, bool UserCanceled)> GetPinFromUser(bool p_isRetry, int p_retriesRemaining)
    //{


    //    if (p_isRetry)
    //    {

    //        await DialogService.ShowAlert(title: "PIN requested", message: $"Invalid PIN. {p_retriesRemaining} retries remaining before PIN is locked.");
    //    }



    //    string? pinEntry = await DialogService.ShowPrompt(title: "PIN requested", message: "Invalid PIN. Please try again.");

    //    if (pinEntry is null)
    //    {
    //        return (null, false);
    //    }

    //    if (pinEntry.Equals("c", StringComparison.InvariantCultureIgnoreCase))
    //    {
    //        return (null, true);
    //    }

    //    return (Encoding.Default.GetBytes(pinEntry), false);
    //}


    //public bool KeyCollectorDelegate(KeyEntryData? pKeyEntryData)
    //{
    //    if (pKeyEntryData is null) return false;

    //    switch (pKeyEntryData.Request)
    //    {
    //        case KeyEntryRequest.Release:
    //            // Do something here if there is sensitive data to clean up.
    //            break;
    //        case KeyEntryRequest.VerifyPivPin:

    //            var (PinBytes, UserCanceled) = await GetPinFromUser(pKeyEntryData.IsRetry, pKeyEntryData.RetriesRemaining ?? 0);

    //            if (UserCanceled || PinBytes is null)
    //            {
    //                return false;
    //            }

    //            pKeyEntryData.SubmitValue(PinBytes);

    //            return true;
    //    }

    //    return false;
    //}







}
