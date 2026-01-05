using PROIECT.Models;

namespace PROIECT;

public partial class LocatarEntryPage : ContentPage
{
   
    public LocatarEntryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var toateApartamentele = await App.Database.GetApartamenteAsync();
        

      
        if (App.UserLogat == null)
        {
            int idLogat = Preferences.Get("LoggedLocatarId", 0);
            if (idLogat != 0) App.UserLogat = await App.Database.GetLocatarAsync(idLogat);
        }

        if (App.UserLogat != null && App.UserLogat.IsAdmin) 
        {
            
            apartamentPicker.ItemsSource = toateApartamentele;
            apartamentPicker.ItemDisplayBinding = new Binding("NumarApartament");
            apartamentPicker.IsEnabled = true;
        }
        else if (App.UserLogat != null)
        {
       
            var apartamentulMeu = toateApartamentele.FirstOrDefault(a => a.ID == App.UserLogat.ApartamentID);

            if (apartamentulMeu != null)
            {
                
                apartamentPicker.ItemsSource = new List<Apartament> { apartamentulMeu };
                apartamentPicker.ItemDisplayBinding = new Binding("NumarApartament");

             
                apartamentPicker.SelectedIndex = 0;

              
                apartamentPicker.IsEnabled = false;
            }
            else
            {
                await DisplayAlert("Eroare", "Contul tău nu este asociat niciunui apartament!", "OK");
            }
        }
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
       
        var apartamentSelectat = apartamentPicker.SelectedItem as Apartament;

        if (apartamentSelectat == null)
        {
            await DisplayAlert("Eroare", "Trebuie selectat un apartament!", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(numeEntry.Text))
        {
            await DisplayAlert("Eroare", "Scrie numele locatarului", "OK");
            return;
        }

        var locatarNou = new Locatar
        {
            Nume = numeEntry.Text,
            Email = emailEntry.Text,
            ApartamentID = apartamentSelectat.ID, 
            IsAdmin = false
        };

        await App.Database.SaveLocatarAsync(locatarNou);
        await DisplayAlert("Succes", "Locatar adăugat!", "OK");
        await Navigation.PopAsync();
    }
}