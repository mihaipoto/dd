using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace TestSR.MAUI.Services;

public static class DialogService
{
    public static void ShowToast(string message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var toast = Toast.Make(message, duration: ToastDuration.Short, textSize: 48);
            await toast.Show();
        });

    }


    public static async Task<string> ShowPrompt(string title, string message)
    {
        return await Shell.Current.DisplayPromptAsync(
            title: title,
            message: message);
    }

    public static async Task ShowAlert(string title, string message)
    {
        await Shell.Current.DisplayAlert(
             title: title,
             message: message,
             cancel: "Okay");
    }


}
