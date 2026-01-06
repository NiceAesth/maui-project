using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class LocatarEntryPage : ContentPage
{
    public LocatarEntryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var toateApartamentele = await App.RestService.RefreshDataAsync();


        var isAdmin = Preferences.Get("IsAdmin", false);
        var myApartamentId = Preferences.Get("LoggedApartamentId", 0);

        if (isAdmin)
        {
            apartamentPicker.ItemsSource = toateApartamentele;
            apartamentPicker.ItemDisplayBinding = new Binding("NumarApartament");
            apartamentPicker.IsEnabled = true;
        }
        else if (myApartamentId != 0)
        {
            var apartamentulMeu = toateApartamentele.FirstOrDefault(a => a.ID == myApartamentId);

            if (apartamentulMeu != null)
            {
                apartamentPicker.ItemsSource = new List<Apartament> { apartamentulMeu };
                apartamentPicker.ItemDisplayBinding = new Binding("NumarApartament");
                apartamentPicker.SelectedIndex = 0;
                apartamentPicker.IsEnabled = false;
            }
            else
            {
                await DisplayAlertAsync("Eroare", "Contul tău nu este asociat niciunui apartament!", "OK");
            }
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var apartamentSelectat = apartamentPicker.SelectedItem as Apartament;

        if (apartamentSelectat == null)
        {
            await DisplayAlertAsync("Eroare", "Trebuie selectat un apartament!", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(numeEntry.Text))
        {
            await DisplayAlertAsync("Eroare", "Scrie numele locatarului", "OK");
            return;
        }

        var locatarNou = new Locatar
        {
            Nume = numeEntry.Text,
            Prenume = "", // UI only has one field for name
            Email = emailEntry.Text,
            ApartamentID = apartamentSelectat.ID
        };

        await App.RestService.SaveLocatarAsync(locatarNou, true);
        await DisplayAlertAsync("Succes", "Locatar adăugat!", "OK");
        await Navigation.PopAsync();
    }
}