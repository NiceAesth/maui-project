using ApartmentManager.Maui.Services;
using Plugin.LocalNotification;

namespace ApartmentManager.Maui;

public partial class App : Application
{
    private static RestService? restService;
    private static NotificationClientService? notificationService;

    public App()
    {
        InitializeComponent();

        var loggedEmail = Preferences.Get("LoggedUserEmail", "");
        if (!string.IsNullOrEmpty(loggedEmail))
        {
            if (Current?.Windows.Count > 0)
                Current.Windows[0].Page = new AppShell();
            else
                MainPage = new AppShell();

            Task.Run(async () =>
            {
                await NotificationService.InitializeAsync();
            });
        }
        else
        {
            var loginPage = new NavigationPage(new LoginPage());
            if (Current?.Windows.Count > 0)
                Current.Windows[0].Page = loginPage;
            else
                MainPage = loginPage;
        }
    }

    public static RestService RestService
    {
        get
        {
            if (restService == null)
                restService = new RestService();
            return restService;
        }
    }

    public static NotificationClientService NotificationService
    {
        get
        {
            if (notificationService == null)
                notificationService = new NotificationClientService();
            return notificationService;
        }
    }
}