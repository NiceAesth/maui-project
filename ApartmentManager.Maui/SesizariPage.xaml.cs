using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class SesizariPage : ContentPage
{
    public SesizariPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


        var sesizari = await App.RestService.RefreshSesizariAsync();
        var locatari = await App.RestService.GetAllLocatariAsync();

        var suntEuAdmin = Preferences.Get("IsAdmin", false);

        foreach (var s in sesizari)
        {
            s.EsteAdmin = suntEuAdmin;

            var autor = locatari.FirstOrDefault(l => l.ID == s.IdLocatar);

            if (autor != null)
                s.NumeAutor = autor.Nume ?? "Locatar";
            else
                s.NumeAutor = "Fost Locatar";
        }

        listView.ItemsSource = null;
        listView.ItemsSource = sesizari;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SesizareEntryPage());
    }

    private async void OnStergeClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var sesizare = button?.BindingContext as Sesizare;

        if (sesizare != null)
        {
            var confirm = await DisplayAlertAsync("Confirmare", "Problema a fost rezolvată?", "Da", "Nu");
            if (confirm)
            {
                await App.RestService.DeleteSesizareAsync(sesizare.ID);
                OnAppearing();
            }
        }
    }
}