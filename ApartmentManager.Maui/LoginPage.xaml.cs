using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = emailEntry.Text;

        if (string.IsNullOrWhiteSpace(email))
        {
            errorLabel.Text = "Vă rugăm să introduceți un email.";
            return;
        }

        var model = new LoginModel
        {
            Email = email,
            Password = passwordEntry.Text
        };

        var token = await App.RestService.LoginAsync(model);

        if (!string.IsNullOrEmpty(token))
        {
            if (Application.Current?.Windows.Count > 0)
                Application.Current.Windows[0].Page = new AppShell();

            await App.NotificationService.InitializeAsync();
        }
        else
        {
            errorLabel.Text = "Contul nu a fost găsit sau parola este incorectă.";
        }
    }

    private async void OnRegisterLabelClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}