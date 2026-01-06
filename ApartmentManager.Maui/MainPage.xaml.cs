using Plugin.LocalNotification;

namespace ApartmentManager.Maui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var request = new NotificationRequest
        {
            NotificationId = 1234,
            Title = "TEST",
            Description = "Salut! Dacă vezi asta, notificările funcționează!",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1)
            }
        };

        await LocalNotificationCenter.Current.Show(request);

        await DisplayAlertAsync("Info", "Am trimis notificarea. Uită-te jos în dreapta!", "OK");
    }
}