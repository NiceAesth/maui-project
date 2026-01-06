using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class SesizareEntryPage : ContentPage
{
    public SesizareEntryPage()
    {
        InitializeComponent();
    }

    async void OnSendClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(subiectEntry.Text) ||
            string.IsNullOrWhiteSpace(descriereEditor.Text))
        {
            await DisplayAlertAsync("Eroare", "Te rugăm să completezi subiectul și descrierea!", "OK");
            return;
        }

        var myId = Preferences.Get("LoggedLocatarId", "");

        var sesizareNoua = new Sesizare
        {
            IdLocatar = myId,           
            Subiect = subiectEntry.Text,
            Descriere = descriereEditor.Text,
            Status = "Nou"
        };

        await App.RestService.SaveSesizareAsync(sesizareNoua, true);

        await DisplayAlertAsync("Mulțumim", "Sesizarea a fost trimisă!", "OK");
        await Navigation.PopAsync();
    }
}