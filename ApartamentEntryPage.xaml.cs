using PROIECT.Models;

namespace PROIECT;

public partial class ApartamentEntryPage : ContentPage
{
    public ApartamentEntryPage()
    {
        InitializeComponent();
    }

   
    protected override void OnAppearing()
    {
        base.OnAppearing();

       
        bool esteAdmin = Preferences.Get("IsAdmin", false);

        
        if (saveBtn != null) saveBtn.IsVisible = esteAdmin;
        if (deleteBtn != null) deleteBtn.IsVisible = esteAdmin;
    }
    

    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var apartament = (Apartament)BindingContext;

        if (!string.IsNullOrWhiteSpace(apartament.NumarApartament))
        {
            bool isNew = (apartament.ID == 0);

            
            await App.Database.SaveApartamentAsync(apartament);

            
            await App.RestService.SaveApartamentAsync(apartament, isNew);

            if (isNew)
            {
                App.TrimiteNotificare("Apartament Nou", $"Apartamentul {apartament.NumarApartament} a fost adăugat în sistem.");
            }

            await DisplayAlert("Succes", "Datele au fost salvate și sincronizate!", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Eroare", "Vă rugăm să introduceți numărul apartamentului!", "OK");
        }
    }

    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var apartament = (Apartament)BindingContext;

        if (apartament.ID != 0)
        {
            bool confirm = await DisplayAlert("Confirmare", "Sigur doriți să ștergeți acest apartament?", "Da", "Nu");

            if (confirm)
            {
                await App.Database.DeleteApartamentAsync(apartament);
                await App.RestService.DeleteApartamentAsync(apartament.ID);
                await Navigation.PopAsync();
            }
        }
    }

    async void OnViewLocatariClicked(object sender, EventArgs e)
    {
        var apartament = BindingContext as Apartament;
        if (apartament != null)
        {
            
            await Navigation.PushAsync(new LocatarPage(apartament));
        }
    }
}