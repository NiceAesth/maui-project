using Plugin.LocalNotification;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

namespace PROIECT;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    async void OnShowMapClicked(object sender, EventArgs e)
    { // <--- ACEASTA ACOLADA LIPSEA SI CAUZA TOATE ERORILE
        try
        {
            double lat = 46.7712;
            double lng = 23.6236;
            string query = "Sediul Asociației";

            string latPunct = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lngPunct = lng.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string url = $"https://www.google.com/maps/search/?api=1&query={latPunct},{lngPunct}";

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                await Launcher.OpenAsync($"geo:0,0?q={latPunct},{lngPunct}({query})");
            }
            else
            {
                await Launcher.OpenAsync(new Uri(url));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Eroare", "Nu s-a putut deschide harta: " + ex.Message, "OK");
        }
    }

    async void OnTestNotificationClicked(object sender, EventArgs e)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

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

        await DisplayAlert("Info", "Notificarea a fost programată să apară în 2 secunde.", "OK");
    }
}