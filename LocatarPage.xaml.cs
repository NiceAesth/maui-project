using PROIECT.Models;

namespace PROIECT;

public partial class LocatarPage : ContentPage
{
    
    Apartament apartamentCurent;

    
    public LocatarPage(Apartament ap)
    {
        InitializeComponent();
        apartamentCurent = ap;
        Title = $"Locatari Ap. {apartamentCurent.NumarApartament}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
      
        listView.ItemsSource = await App.Database.GetLocatariByApartamentAsync(apartamentCurent.ID);
    }


    async void OnLocatarAddedClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new LocatarEntryPage());
    }

    
    async void OnDeleteLocatarClicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var locatar = menuItem.BindingContext as Locatar;

        if (locatar != null)
        {
            bool confirm = await DisplayAlert("Confirmare", $"Ștergi pe {locatar.Nume}?", "Da", "Nu");
            if (confirm)
            {
                await App.Database.DeleteLocatarAsync(locatar);
                OnAppearing(); // Refresh la listă
            }
        }
    }
}