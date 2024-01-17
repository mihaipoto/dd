using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;

namespace TestSR.MAUI.Services;

public interface IClientCertificateProvider
{
    X509Certificate2Collection GetClientCertificates(string subject);
    //X509Certificate2Collection GetClientCertificates();

    //GetPinFromUserHAndler GetPinHandler { get; set; }

    Func<KeyEntryData, bool> Functie { get; set; }


}




public class ClientCertificateProvider : IClientCertificateProvider
{

    public Func<KeyEntryData, bool> Functie { get; set; }
    //public GetPinFromUserHAndler GetPinHandler { get; set; }

    public X509Certificate2Collection GetClientCertificates(string subject)
    {
        var localMachineStore = new X509Store(StoreLocation.LocalMachine);
        localMachineStore.Open(OpenFlags.ReadOnly);
        var certificates = localMachineStore.Certificates.Find(findType: X509FindType.FindBySubjectName,
                                                               findValue: subject,
                                                               validOnly: false) ?? [];
        localMachineStore.Close();
        var cert = certificates[0];

        return certificates;
    }

    //public X509Certificate2Collection GetClientCertificates(string subject)
    //{
    //    X509Certificate2 cert = new(
    //       fileName: "E:\\certs\\u1.pfx", password: "12345");

    //    X509Certificate2Collection col = [cert];
    //    return col;
    //}




    //public X509Certificate2Collection GetClientCertificates()
    //{


    //    X509Certificate2 cert = new(
    //       fileName: "E:\\certs\\u1.pfx", password: "12345");

    //    X509Certificate2Collection col = [cert];
    //    return col;
    //}

    //public X509Certificate2Collection GetClientCertificates()
    //{
    //    //DialogService.ShowToast("Initiating PIV Session, please wait...");
    //    var yubiKey = YubiKeyDevice.FindAll().
    //        FirstOrDefault(p_device => p_device.HasFeature(YubiKeyFeature.PivEccP256));

    //    var session = yubiKey is null
    //        ? throw new CryptographicException("An appropriate YubiKey could not be found on this system.")
    //        : new PivSession(yubiKey);
    //    //DialogService.ShowToast("Session Created...");

    //    //session.KeyCollector = Functie;
    //    //session.VerifyPin();
    //    DialogService.ShowToast(session.PinVerified ? "Pin verificat" : "Pin neverificat");
    //    X509Certificate2 cert = session.GetCertificate(PivSlot.Authentication);
    //    X509Certificate2Collection col = [cert];
    //    return col;
    //}






}
