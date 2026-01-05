using PROIECT.Models;

namespace PROIECT;

public partial class RegisterPage : ContentPage
{
  
    List<Apartament> listaApartamente;

    public RegisterPage()
    {
        InitializeComponent();
    }

 
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        
        listaApartamente = await App.Database.GetApartamenteAsync();

        if (listaApartamente.Count == 0)
        {
            var adminAp = new Apartament
            {
                NumarApartament = "Birou Admin",
                Etaj = 0,
                Suprafata = 0
            };
            await App.Database.SaveApartamentAsync(adminAp);

           
            listaApartamente = await App.Database.GetApartamenteAsync();
        }

        
        apartamentPicker.Items.Clear();
        foreach (var ap in listaApartamente)
        {
            apartamentPicker.Items.Add($"Ap. {ap.NumarApartament} (Et. {ap.Etaj})");
        }
    }

    async void OnRegisterClicked(object sender, EventArgs e)
    {
      
        if (string.IsNullOrWhiteSpace(numeEntry.Text) ||
            string.IsNullOrWhiteSpace(emailEntry.Text) ||
            string.IsNullOrWhiteSpace(passwordEntry.Text))
        {
            await DisplayAlert("Eroare", "Toate câmpurile sunt obligatorii!", "OK");
            return;
        }

        if (passwordEntry.Text != confirmPasswordEntry.Text)
        {
            await DisplayAlert("Eroare", "Parolele nu coincid!", "OK");
            return;
        }

        if (apartamentPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Eroare", "Te rugăm să alegi un apartament din listă!", "OK");
            return;
        }

      
        var apartamentAles = listaApartamente[apartamentPicker.SelectedIndex];

      
        var locatarNou = new Locatar
        {
            Nume = numeEntry.Text,
            Email = emailEntry.Text,
            ApartamentID = apartamentAles.ID, 
            IsAdmin = adminCheckBox.IsChecked
        };

        
        await App.Database.SaveLocatarAsync(locatarNou);
        await App.RestService.SaveLocatarAsync(locatarNou, true);

        await DisplayAlert("Succes", "Cont creat! Te poți autentifica.", "OK");

       
        await Navigation.PopAsync();
    }
}