using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class ApartamentEntryPage : ContentPage
{
    public ApartamentEntryPage()
    {
        InitializeComponent();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();


        var esteAdmin = Preferences.Get("IsAdmin", false);


        if (saveBtn != null) saveBtn.IsVisible = esteAdmin;
        if (deleteBtn != null) deleteBtn.IsVisible = esteAdmin;
    }


    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var apartament = BindingContext as Apartament;
        if (apartament == null) return;

        if (!string.IsNullOrWhiteSpace(apartament.NumarApartament))
        {
            var isNew = apartament.ID == 0;

            await App.RestService.SaveApartamentAsync(apartament, isNew);

            await DisplayAlertAsync("Succes", "Datele au fost salvate și sincronizate!", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlertAsync("Eroare", "Vă rugăm să introduceți numărul apartamentului!", "OK");
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var apartament = BindingContext as Apartament;
        if (apartament == null) return;

        if (apartament.ID != 0)
        {
            var confirm =
                await DisplayAlertAsync("Confirmare", "Sigur doriți să ștergeți acest apartament?", "Da", "Nu");

            if (confirm)
            {
                await App.RestService.DeleteApartamentAsync(apartament.ID);
                await Navigation.PopAsync();
            }
        }
    }

    private async void OnViewLocatariClicked(object sender, EventArgs e)
    {
        var apartament = BindingContext as Apartament;
        if (apartament != null) await Navigation.PushAsync(new LocatarPage(apartament));
    }
}