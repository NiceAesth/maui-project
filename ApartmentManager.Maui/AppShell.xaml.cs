namespace ApartmentManager.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }


    private void OnLogoutClicked(object sender, EventArgs e)
    {
        Preferences.Clear();

        if (Application.Current?.Windows.Count > 0)
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage());
    }
}