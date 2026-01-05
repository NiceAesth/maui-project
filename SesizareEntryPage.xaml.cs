using PROIECT.Models;

namespace PROIECT;

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
            await DisplayAlert("Eroare", "Te rugăm să completezi subiectul și descrierea!", "OK");
            return;
        }

        
        int idulMeu = Preferences.Get("LoggedLocatarId", 0);

       
        var sesizareNoua = new Sesizare
        {
            IdLocatar = idulMeu,           
            Subiect = subiectEntry.Text,
            Descriere = descriereEditor.Text,
            Status = "Deschis"             
        };

        
        await App.Database.SaveSesizareAsync(sesizareNoua);

       
        await DisplayAlert("Mulțumim", "Sesizarea a fost trimisă!", "OK");
        await Navigation.PopAsync();
    }
}