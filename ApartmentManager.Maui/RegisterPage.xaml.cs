using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class RegisterPage : ContentPage
{
    private List<Apartament>? listaApartamente;

    public RegisterPage()
    {
        InitializeComponent();
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();


        listaApartamente = await App.RestService.RefreshDataAsync();

        if (listaApartamente == null || listaApartamente.Count == 0) return;


        apartamentPicker.Items.Clear();
        foreach (var ap in listaApartamente) apartamentPicker.Items.Add($"Ap. {ap.NumarApartament} (Et. {ap.Etaj})");
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(numeEntry.Text) ||
            string.IsNullOrWhiteSpace(emailEntry.Text) ||
            string.IsNullOrWhiteSpace(passwordEntry.Text) ||
            listaApartamente == null)
        {
            await DisplayAlertAsync("Eroare", "Toate câmpurile sunt obligatorii!", "OK");
            return;
        }

        if (passwordEntry.Text != confirmPasswordEntry.Text)
        {
            await DisplayAlertAsync("Eroare", "Parolele nu coincid!", "OK");
            return;
        }

        if (apartamentPicker.SelectedIndex == -1)
        {
            await DisplayAlertAsync("Eroare", "Te rugăm să alegi un apartament din listă!", "OK");
            return;
        }

        var model = new RegisterModel
        {
            Email = emailEntry.Text,
            Password = passwordEntry.Text,
            ConfirmPassword = confirmPasswordEntry.Text,
            Nume = numeEntry.Text,
            Prenume = "", // UI only has Name entry, splitting or leaving empty
            ApartamentID = listaApartamente[apartamentPicker.SelectedIndex].ID
        };

        var success = await App.RestService.RegisterAsync(model);

        if (success)
        {
            await DisplayAlertAsync("Succes", "Cont creat! Te poți autentifica.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlertAsync("Eroare", "Înregistrarea a eșuat. Verifică datele introduse.", "OK");
        }
    }
}