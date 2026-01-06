using System.Globalization;
using Plugin.LocalNotification;

namespace ApartmentManager.Maui;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private async void OnShowMapClicked(object sender, EventArgs e)
    {
        try
        {
            var lat = 46.7712;
            var lng = 23.6236;
            var query = "Sediul Asociației";

            var latPunct = lat.ToString(CultureInfo.InvariantCulture);
            var lngPunct = lng.ToString(CultureInfo.InvariantCulture);

            var url = $"https://www.google.com/maps/search/?api=1&query={latPunct},{lngPunct}";

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                await Launcher.OpenAsync($"geo:0,0?q={latPunct},{lngPunct}({query})");
            else
                await Launcher.OpenAsync(new Uri(url));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Eroare", "Nu s-a putut deschide harta: " + ex.Message, "OK");
        }
    }

    private async void OnTestNotificationClicked(object sender, EventArgs e)
    {
        if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
            await LocalNotificationCenter.Current.RequestNotificationPermission();

        var request = new NotificationRequest
        {
            NotificationId = 1000,
            Title = "Reamintire Asociație",
            Description = "Aceasta este o notificare de test. Aplicația funcționează!",
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(2)
            }
        };

        await LocalNotificationCenter.Current.Show(request);

        await DisplayAlertAsync("Info", "Notificarea a fost programată să apară în 2 secunde.", "OK");
    }
}