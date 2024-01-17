using Microsoft.Extensions.Logging;
using Yubico.YubiKey;

namespace TestSR.MAUI.Services;

public class YubikeyDisconnectionDetectorService
{
    private readonly ILogger<YubikeyDisconnectionDetectorService> _logger;
    private YubiKeyDeviceListener _listener;

    public YubikeyDisconnectionDetectorService(ILogger<YubikeyDisconnectionDetectorService> logger)
    {
        _logger = logger;
        _listener = YubiKeyDeviceListener.Instance;
        _listener.Removed += _listener_Removed;


    }

    private void _listener_Removed(object? sender, YubiKeyDeviceEventArgs e)
    {

    }
}
