using ApartmentManager.Maui.Services;

namespace ApartmentManager.Maui;

public partial class App
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var loggedEmail = Preferences.Get("LoggedUserEmail", "");
        Page rootPage;

        if (!string.IsNullOrEmpty(loggedEmail))
        {
            rootPage = new AppShell();

            Task.Run(async () =>
            {
                await NotificationService.InitializeAsync();
            });
        }
        else
        {
            rootPage = new NavigationPage(new LoginPage());
        }

        return new Window(rootPage);
    }

    public static RestService RestService
    {
        get
        {
            field ??= new RestService();
            return field;
        }
    }

    public static NotificationClientService NotificationService
    {
        get
        {
            field ??= new NotificationClientService();
            return field;
        }
    }
}