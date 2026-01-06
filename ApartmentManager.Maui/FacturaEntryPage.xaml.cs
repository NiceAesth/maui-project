using ApartmentManager.Shared.Models;

namespace ApartmentManager.Maui;

public partial class FacturaEntryPage : ContentPage
{
    private FacturaIndividuala? _facturaDeModificat;
    private List<Apartament>? listaApartamente;


    public FacturaEntryPage(FacturaIndividuala? factura = null)
    {
        InitializeComponent();
        _facturaDeModificat = factura;

        if (_facturaDeModificat != null) Title = "Modifică Factura";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        listaApartamente = await App.RestService.RefreshDataAsync();
        apartamentPicker.Items.Clear();
        foreach (var ap in listaApartamente) apartamentPicker.Items.Add(ap.NumarApartament);


        if (_facturaDeModificat != null)
        {
            sumaEntry.Text = _facturaDeModificat.SumaDePlata.ToString();


            tipPicker.SelectedItem = _facturaDeModificat.Tip;


            var apartamentVechi = listaApartamente.FirstOrDefault(x => x.ID == _facturaDeModificat.IdApartament);
            if (apartamentVechi != null) apartamentPicker.SelectedIndex = listaApartamente.IndexOf(apartamentVechi);
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (apartamentPicker.SelectedIndex == -1 || tipPicker.SelectedIndex == -1 ||
            string.IsNullOrWhiteSpace(sumaEntry.Text) || listaApartamente == null)
        {
            await DisplayAlertAsync("Eroare", "Selectați apartamentul, tipul facturii și suma!", "OK");
            return;
        }

        var apartamentSelectat = listaApartamente[apartamentPicker.SelectedIndex];


        var isNew = false;
        if (_facturaDeModificat == null)
        {
            _facturaDeModificat = new FacturaIndividuala();
            isNew = true;
        }


        _facturaDeModificat!.IdApartament = apartamentSelectat.ID;
        _facturaDeModificat.NumeApartament = apartamentSelectat.NumarApartament;
        _facturaDeModificat.SumaDePlata = decimal.Parse(sumaEntry.Text);
        _facturaDeModificat.Luna = datePicker.Date.ToString() ?? "";
        _facturaDeModificat.Tip = tipPicker.SelectedItem?.ToString() ?? "";


        if (string.IsNullOrEmpty(_facturaDeModificat.Status)) _facturaDeModificat.Status = "Neplătit";


        await App.RestService.SaveFacturaAsync(_facturaDeModificat, isNew);

        await DisplayAlertAsync("Succes", "Factura a fost salvată!", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}