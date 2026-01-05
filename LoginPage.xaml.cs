using PROIECT.Models;

namespace PROIECT;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = emailEntry.Text;

        if (string.IsNullOrWhiteSpace(email))
        {
            errorLabel.Text = "Vă rugăm să introduceți un email.";
            return;
        }

        var locatari = await App.Database.GetLocatariAsync();

        
        var locatar = locatari.FirstOrDefault(l => l.Email != null && l.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (locatar != null)
        {
            
            Preferences.Set("LoggedLocatarId", locatar.ID);
            Preferences.Set("LoggedApartamentId", locatar.ApartamentID);
            Preferences.Set("LoggedLocatarName", locatar.Nume);
            Preferences.Set("IsAdmin", locatar.IsAdmin);

           
            App.UserLogat = locatar;
         

            Application.Current.MainPage = new AppShell();
        }
        else
        {
            errorLabel.Text = "Contul nu a fost găsit. Verificați emailul sau înregistrați-vă.";
        }
    }

    async void OnRegisterLabelClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}