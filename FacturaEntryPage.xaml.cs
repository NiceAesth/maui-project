using PROIECT.Models;
using System.Globalization;

namespace PROIECT;

public partial class FacturaEntryPage : ContentPage
{
    List<Apartament> listaApartamente;
    FacturaIndividuala _facturaDeModificat; 

    
    public FacturaEntryPage(FacturaIndividuala factura = null)
    {
        InitializeComponent();
        _facturaDeModificat = factura;

        if (_facturaDeModificat != null)
        {
            Title = "Modifică Factura"; 
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        listaApartamente = await App.Database.GetApartamenteAsync();
        apartamentPicker.Items.Clear();
        foreach (var ap in listaApartamente)
        {
            apartamentPicker.Items.Add(ap.NumarApartament);
        }

     
        if (_facturaDeModificat != null)
        {
            
            sumaEntry.Text = _facturaDeModificat.SumaDePlata.ToString();

            
            tipPicker.SelectedItem = _facturaDeModificat.Tip;

            
            var apartamentVechi = listaApartamente.FirstOrDefault(x => x.ID == _facturaDeModificat.IdApartament);
            if (apartamentVechi != null)
            {
                apartamentPicker.SelectedIndex = listaApartamente.IndexOf(apartamentVechi);
            }
        }
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        if (apartamentPicker.SelectedIndex == -1 || tipPicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(sumaEntry.Text))
        {
            await DisplayAlert("Eroare", "Selectați apartamentul, tipul facturii și suma!", "OK");
            return;
        }

        var apartamentSelectat = listaApartamente[apartamentPicker.SelectedIndex];

        
        if (_facturaDeModificat == null)
        {
            _facturaDeModificat = new FacturaIndividuala();
        }

  
        _facturaDeModificat.IdApartament = apartamentSelectat.ID;
        _facturaDeModificat.NumeApartament = apartamentSelectat.NumarApartament;
        _facturaDeModificat.SumaDePlata = decimal.Parse(sumaEntry.Text);
        _facturaDeModificat.Luna = datePicker.Date.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
        _facturaDeModificat.Tip = tipPicker.SelectedItem.ToString();

       
        if (string.IsNullOrEmpty(_facturaDeModificat.Status))
        {
            _facturaDeModificat.Status = "Neplătit";
        }

      
        await App.Database.SaveFacturaAsync(_facturaDeModificat);

        await DisplayAlert("Succes", "Factura a fost salvată!", "OK");
        await Navigation.PopAsync();
    }

    async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}