



using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;


await Task.Delay(2000);
var httpClient = BuildHttpClient();

if (httpClient != null)
{
    char userOption = ' ';
    while (userOption != 'q')
    {
        Console.Write("Choose option: ");
        userOption = Console.ReadKey().KeyChar;
        Console.Clear();
        switch (userOption)
        {
            case 'g':
                await httpClient.GetRequest();
                break;
            case 'i':
                GetMetadata();
                break;
            default:
                break;
        }
    }
}
else
{
    Console.ReadKey();
}









HttpClient? BuildHttpClient()
{
    try
    {
        var certs = GetClientCertificatesFromStore("u1");
        //var certs = GetClientCertificatesFromYubikey();
        //var certs = GetClientCertificatesFromDisk();
        var _sslOptions = new SslClientAuthenticationOptions()
        {
            ClientCertificates = certs,
            RemoteCertificateValidationCallback = (a, b, n, c) =>
            {
                return true;
            }
        };
        var _socksHttpHandler = new SocketsHttpHandler
        {
            SslOptions = _sslOptions
        };
        var httpClient = HttpClientFactory.Create(_socksHttpHandler);
        return httpClient;
    }
    catch (Exception ex)
    {

        Console.WriteLine(ex.Message);
        return default;
    }

}


X509Certificate2Collection GetClientCertificatesFromDisk()
{
    var cert = new X509Certificate2("E:\\certs\\u1.pfx", "12345");
    var certificates = new X509Certificate2Collection();
    _ = certificates.Add(cert);

    return certificates;
}

X509Certificate2Collection GetClientCertificatesFromStore(string subject)
{
    var localMachineStore = new X509Store(StoreLocation.CurrentUser);
    localMachineStore.Open(OpenFlags.ReadOnly);
    var certificates = localMachineStore.Certificates.Find(findType: X509FindType.FindBySubjectName,
                                                           findValue: subject,
                                                           validOnly: false) ?? [];
    localMachineStore.Close();
    if (certificates.Count != 1)
    {
        throw new Exception("No client certificate found");
    }
    var cert = certificates[0];

    return certificates;
}

X509Certificate2Collection GetClientCertificatesFromYubikey()
{
    Console.WriteLine("Initiating PIV Session, please wait...");
    var yubiKey = YubiKeyDevice.FindAll().
        FirstOrDefault(p_device => p_device.HasFeature(YubiKeyFeature.PivApplication));

    using var session = yubiKey is null
        ? throw new CryptographicException("An appropriate YubiKey could not be found on this system.")
        : new PivSession(yubiKey);

    //session.KeyCollector = (r) => { r.SubmitValue(Encoding.Default.GetBytes("123456")); return true; };
    //session.VerifyPin();
    //Console.WriteLine(session.PinVerified ? "Pin verificat" : "Pin neverificat");

    X509Certificate2 cert = session.GetCertificate(PivSlot.Authentication);
    X509Certificate2Collection col = [cert];
    return col;
}


void GetMetadata()
{
    Console.WriteLine("Initiating PIV Session, please wait...");
    var yubiKey = YubiKeyDevice.FindAll().
        FirstOrDefault(p_device => p_device.HasFeature(YubiKeyFeature.PivApplication));

    using var session = yubiKey is null
        ? throw new CryptographicException("An appropriate YubiKey could not be found on this system.")
        : new PivSession(yubiKey);
    var metadata = session.GetMetadata(PivSlot.Authentication);
    Console.WriteLine($"pin policy: {metadata.PinPolicy} {Environment.NewLine} " +
        $"algoritm {metadata.Algorithm} {Environment.NewLine} " +
        $"touch policy {metadata.TouchPolicy} {Environment.NewLine}" +
        $"key status{metadata.KeyStatus}");
}


static class HttpClientExtension
{
    public static async Task GetRequest(this HttpClient httpClient)
    {
        try
        {
            var httpResponseMessage = await httpClient.GetAsync("https://localhost:5555/hello");

            if (httpResponseMessage.IsSuccessStatusCode)
            {

                var resp = await httpResponseMessage.Content.ReadAsStringAsync();
                await Console.Out.WriteLineAsync(resp);
            }
            else
            {
                await Console.Out.WriteLineAsync($"{httpResponseMessage.StatusCode}");
            }
        }
        catch (Exception ex)
        {

            await Console.Out.WriteLineAsync(ex.ToString());
        }

    }
}


