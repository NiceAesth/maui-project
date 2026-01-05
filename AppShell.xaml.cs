namespace PROIECT;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    
    private void OnLogoutClicked(object sender, EventArgs e)
    {
   

       
        Preferences.Clear();

        App.UserLogat = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}