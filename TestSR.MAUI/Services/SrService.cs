using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using static TestSR.MAUI.Handlers;


namespace TestSR.MAUI.Services;


public enum KeyedPolicies
{
    MyPolicy
}

public class SrService()
{
    enum HubMethods
    {
        ReceiveNotification,
        UpdateTime
    }

    private HubConnection? _hubConnection;



    public async Task ConnectToHub()
    {
        try
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                .WithUrl(Opt.Url, (options) =>
                {
                    options = Opt.ConnectionOptions;
                })

                .WithAutomaticReconnect(Opt.RetryPolicy)
                .Build();

                _hubConnection.On<string>(HubMethods.ReceiveNotification.ToString(), message =>
                {
                    Opt.Events.ReceiveNotificationHandler(message);
                });

                _hubConnection.On<DateTime>(HubMethods.UpdateTime.ToString(), time =>
                {
                    Opt.Events.ReceiveTimenHandler(time);
                });
                _hubConnection.Closed += HubConnection_Closed;
                _hubConnection.Reconnected += HubConnection_Reconnected;
                _hubConnection.HandshakeTimeout = TimeSpan.FromSeconds(60);

            }
            if (_hubConnection.State.Equals(HubConnectionState.Disconnected))
            {
                await _hubConnection.StartAsync();
            }
        }
        catch (Exception ex)
        {
            Opt.Events.ExceptionHandler(ex);
        }
    }

    private Task HubConnection_Reconnected(string? arg)
    {
        Opt.Events.ReconnectedHandler();
        return Task.CompletedTask;
    }

    private Task HubConnection_Closed(Exception? arg)
    {
        Opt.Events.ConnectionClosedHandler(arg);
        return Task.CompletedTask;
    }

    private SrServiceOptions Opt { get; set; } = new();

    public void ConfigureService(Action<SrServiceOptions> configureOptions)
    {
        configureOptions(Opt);
    }

}


public class SrServiceOptions
{
    public string Url { get; set; } = string.Empty;
    public IRetryPolicy RetryPolicy { get; set; } = new MyRetryPolicy();
    public HttpConnectionOptions ConnectionOptions { get; set; } = new();
    public SrServiceEvents Events { get; set; } = new();
}

public class SrServiceEvents
{
    public ExceptionHandler ExceptionHandler { get; set; } = (_) => { };
    public ReceiveNotificationHandler ReceiveNotificationHandler { get; set; } = (_) => { };
    public ReceiveTimeHandler ReceiveTimenHandler { get; set; } = (_) => { };
    public ConnectionClosedHandler ConnectionClosedHandler { get; set; } = (_) => { };
    public ReconnectedHandler ReconnectedHandler { get; set; } = () => { };
}


public class MyRetryPolicy() : IMyRetryPolicy
{
    private TimeSpan _retryInterval = TimeSpan.FromSeconds(2);

    public RetryHandler? RetryHandler { get; set; }

    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        if (RetryHandler is not null) { RetryHandler(retryContext); }
        return _retryInterval;
    }
}

public static class MyRetryPolicyExtensions
{
    public static IMyRetryPolicy HandleReconnectionAttempt(this IMyRetryPolicy policy, RetryHandler? retryHandler)
    {
        policy.RetryHandler = retryHandler;
        return policy;
    }
}

public interface IMyRetryPolicy : IRetryPolicy
{
    RetryHandler? RetryHandler { get; set; }
}

