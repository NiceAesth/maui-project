using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class ApartamentPage : ContentPage
{
    public ApartamentPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


        listView.ItemsSource = await App.RestService.RefreshDataAsync();


        var esteAdmin = Preferences.Get("IsAdmin", false);
        //await DisplayAlertAsync("Debug", "Ești Admin: " + esteAdmin, "OK");

        if (addApartamentBtn != null) addApartamentBtn.IsVisible = esteAdmin;
    }


    private async void OnApartamentAddedClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ApartamentEntryPage
        {
            BindingContext = new Apartament()
        });
    }


    private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Apartament selectedApartament)
        {
            ((CollectionView)sender).SelectedItem = null;

            await Navigation.PushAsync(new ApartamentEntryPage
            {
                BindingContext = selectedApartament
            });
        }
    }
}