namespace PROIECT;

public partial class LogoutPage : ContentPage
{
    public LogoutPage()
    {
        InitializeComponent();
    }

    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        Preferences.Clear();

      
        App.UserLogat = null;

     
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}