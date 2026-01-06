using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class LocatarPage : ContentPage
{
    private readonly Apartament? apartamentCurent;


    public LocatarPage(Apartament ap)
    {
        InitializeComponent();
        apartamentCurent = ap;
        Title = $"Locatari Ap. {apartamentCurent.NumarApartament}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (apartamentCurent != null)
            listView.ItemsSource = await App.RestService.GetLocatariByApartamentAsync(apartamentCurent.ID);
    }


    private async void OnLocatarAddedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LocatarEntryPage());
    }


    private async void OnDeleteLocatarClicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var locatar = menuItem?.BindingContext as Locatar;

        if (locatar != null && !string.IsNullOrEmpty(locatar.ID))
        {
            var confirm = await DisplayAlertAsync("Confirmare", $"Ștergi pe {locatar.Nume}?", "Da", "Nu");
            if (confirm)
            {
                await App.RestService.DeleteLocatarAsync(locatar.ID);
                OnAppearing();
            }
        }
    }
}