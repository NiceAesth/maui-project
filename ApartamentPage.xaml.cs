using PROIECT.Models;

namespace PROIECT;

public partial class ApartamentPage : ContentPage
{
    public ApartamentPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

       
        listView.ItemsSource = await App.Database.GetApartamenteAsync();

       
    
        bool esteAdmin = Preferences.Get("IsAdmin", false);
        //await DisplayAlert("Debug", "Ești Admin: " + esteAdmin, "OK");

        if (addApartamentBtn != null)
        {
            addApartamentBtn.IsVisible = esteAdmin;
        }
    }


    async void OnApartamentAddedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ApartamentEntryPage
        {
            BindingContext = new Apartament()
        });
    }

   
    async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            await Navigation.PushAsync(new ApartamentEntryPage
            {
                BindingContext = e.SelectedItem as Apartament
            });
        }
    }
}