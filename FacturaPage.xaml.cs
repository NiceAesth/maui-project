using PROIECT.Models;

namespace PROIECT;

public partial class FacturaPage : ContentPage
{
    public FacturaPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

       
        if (App.UserLogat == null) return;

        bool suntEuAdmin = App.UserLogat.IsAdmin;
        int idApartamentulMeu = App.UserLogat.ApartamentID;
     

        List<FacturaIndividuala> facturi;

        if (suntEuAdmin)
        {
       
            facturi = await App.Database.GetFacturiAsync();
        }
        else
        {
           
            facturi = await App.Database.GetFacturiByApartamentAsync(idApartamentulMeu);
        }

 
        if (facturi.Count == 0)
        {
            listView.IsVisible = false;
            emptyLabel.IsVisible = true;

            if (suntEuAdmin) emptyLabel.Text = "Nu a fost emisă nicio factură.";
            else emptyLabel.Text = "Nu ai facturi de plată. Felicitări!";
        }
        else
        {
            listView.IsVisible = true;
            emptyLabel.IsVisible = false;
        }

        if (facturi.Count > 0)
        {
            var apartamente = await App.Database.GetApartamenteAsync();

            foreach (var f in facturi)
            {
              
                f.EsteAdmin = suntEuAdmin;

                var ap = apartamente.FirstOrDefault(a => a.ID == f.IdApartament);
                string numarAp = (ap != null) ? ap.NumarApartament : "?";

                f.DetaliiAfisare = $"Ap. {numarAp} - {f.Luna}";
            }
        }

        listView.ItemsSource = null;
        listView.ItemsSource = facturi;

      
        if (addFacturaBtn != null)
        {
            addFacturaBtn.IsVisible = suntEuAdmin;
        }
    }

    async void OnAddFacturaClicked(object sender, EventArgs e)
    {
       
        await Navigation.PushAsync(new FacturaEntryPage());
    }

    async void OnPlatesteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var factura = button.BindingContext as FacturaIndividuala;

        bool confirm = await DisplayAlert("Plată", $"Achitați {factura.SumaDePlata} RON?", "Da", "Nu");
        if (confirm)
        {
            factura.Status = "Plătit"; 
            await App.Database.SaveFacturaAsync(factura);
            OnAppearing(); 
            await DisplayAlert("Succes", "Plată înregistrată!", "OK");
        }
    }

    async void OnStergeClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var factura = button.BindingContext as FacturaIndividuala;

        bool confirm = await DisplayAlert("Ștergere", "Sigur ștergi această factură?", "Da", "Nu");
        if (confirm)
        {
            await App.Database.DeleteFacturaAsync(factura);
            OnAppearing(); 
        }
    }

    async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var factura = button.BindingContext as FacturaIndividuala;

        if (factura != null)
        {
            await Navigation.PushAsync(new FacturaEntryPage(factura));
        }
    }
}