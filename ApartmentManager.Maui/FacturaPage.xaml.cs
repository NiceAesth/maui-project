using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class FacturaPage : ContentPage
{
    public FacturaPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


        var suntEuAdmin = Preferences.Get("IsAdmin", false);
        var idApartamentulMeu = Preferences.Get("LoggedApartamentId", 0);


        List<FacturaIndividuala> facturi;

        if (suntEuAdmin)
            facturi = await App.RestService.GetAllFacturiAsync();
        else
            facturi = await App.RestService.RefreshFacturiAsync(idApartamentulMeu);


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
            var apartamente = await App.RestService.RefreshDataAsync();

            foreach (var f in facturi)
            {
                f.EsteAdmin = suntEuAdmin;

                var ap = apartamente.FirstOrDefault(a => a.ID == f.IdApartament);
                var numarAp = ap?.NumarApartament ?? "?";

                f.DetaliiAfisare = $"Ap. {numarAp} - {f.Luna}";
            }
        }

        listView.ItemsSource = null;
        listView.ItemsSource = facturi;


        if (addFacturaBtn != null) addFacturaBtn.IsVisible = suntEuAdmin;
    }

    private async void OnAddFacturaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FacturaEntryPage());
    }

    private async void OnPlatesteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var factura = button?.BindingContext as FacturaIndividuala;

        if (factura != null)
        {
            var confirm = await DisplayAlertAsync("Plată", $"Achitați {factura.SumaDePlata} RON?", "Da", "Nu");
            if (confirm)
            {
                factura.Status = "Plătit";
                await App.RestService.SaveFacturaAsync(factura, false);
                OnAppearing();
                await DisplayAlertAsync("Succes", "Plată înregistrată!", "OK");
            }
        }
    }

    private async void OnStergeClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var factura = button?.BindingContext as FacturaIndividuala;

        if (factura != null)
        {
            var confirm = await DisplayAlertAsync("Ștergere", "Sigur ștergi această factură?", "Da", "Nu");
            if (confirm)
            {
                await App.RestService.DeleteFacturaAsync(factura.ID);
                OnAppearing();
            }
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var factura = button?.BindingContext as FacturaIndividuala;

        if (factura != null) await Navigation.PushAsync(new FacturaEntryPage(factura));
    }
}