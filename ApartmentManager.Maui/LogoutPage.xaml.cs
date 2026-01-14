namespace ApartmentManager.Maui;

public partial class LogoutPage : ContentPage
{
    public LogoutPage()
    {
        InitializeComponent();
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        Preferences.Clear();
        await App.NotificationService.StopAsync();

        if (Application.Current?.Windows.Count > 0)
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
    }
}