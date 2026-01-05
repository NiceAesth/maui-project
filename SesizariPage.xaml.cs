using PROIECT.Models;

namespace PROIECT;

public partial class SesizariPage : ContentPage
{
    public SesizariPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        
        var sesizari = await App.Database.GetSesizariAsync();
        var locatari = await App.Database.GetLocatariAsync();

       
        bool suntEuAdmin = Preferences.Get("IsAdmin", false);

       
        foreach (var s in sesizari)
        {
           
            s.EsteAdmin = suntEuAdmin;

           
            var autor = locatari.FirstOrDefault(l => l.ID == s.IdLocatar);

            if (autor != null)
            {
                s.NumeAutor = autor.Nume; 
            }
            else
            {
                s.NumeAutor = "Fost Locatar"; 
            }
        }

       
        listView.ItemsSource = null;
        listView.ItemsSource = sesizari;
    }

   
    async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SesizareEntryPage());
    }

   
    async void OnStergeClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var sesizare = button.BindingContext as Sesizare;

        bool confirm = await DisplayAlert("Confirmare", "Problema a fost rezolvată?", "Da", "Nu");
        if (confirm)
        {
            await App.Database.DeleteSesizareAsync(sesizare);
            OnAppearing();
        }
    }
}