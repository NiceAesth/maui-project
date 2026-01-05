using PROIECT.Models;
using Plugin.LocalNotification;
using System.Linq;

namespace PROIECT
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // TEST 1: Vedem dacă pagina se încarcă
            // await DisplayAlert("DEBUG", "Pagina s-a încărcat!", "OK"); 

            if (App.UserLogat == null)
            {
                await DisplayAlert("EROARE CRITICĂ", "App.UserLogat este NULL! Nu s-a făcut salvarea la Login.", "OK");
                return;
            }

            await VerificaSiTrimiteNotificari();
        }

        private async Task VerificaSiTrimiteNotificari()
        {
            var userCurent = App.UserLogat;
            if (userCurent == null) return;

            if (userCurent.IsAdmin)
            {
                // === LOGICA ADMIN ===
                var sesizari = await App.Database.GetSesizariAsync();
                if (sesizari.Count > 0)
                {
                    TrimiteNotificare("Admin", $"Ai {sesizari.Count} sesizări.");
                }
            }
            else
            {
                // === LOGICA LOCATAR (AICI VERIFICĂM) ===
                var facturi = await App.Database.GetFacturiByApartamentAsync(userCurent.ApartamentID);

                // 1. Construim un mesaj să vedem ce a găsit în baza de date
                string mesajDebug = $"Am găsit {facturi.Count} facturi totale pentru acest apartament.\n";

                foreach (var f in facturi)
                {
                    mesajDebug += $"- Factura: {f.Tip}, Statusul din Bază: '{f.Status}'\n";
                }

                // 2. Afișăm fereastra de diagnostic pe ecran
                await DisplayAlert("DIAGNOSTIC FACTURI", mesajDebug, "OK");

                // 3. Facem verificarea mai relaxată (să nu conteze litere mici/mari)
                // Căutăm orice status care NU este "Plătit" și nici "Platit"
                var facturiNeplatite = facturi.Where(f =>
                    !string.Equals(f.Status, "Plătit", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(f.Status, "Platit", StringComparison.OrdinalIgnoreCase)
                ).ToList();

                if (facturiNeplatite.Count > 0)
                {
                    TrimiteNotificare("Facturi Scadente", $"Ai {facturiNeplatite.Count} facturi neplătite!");
                }
            }
        }

        private void TrimiteNotificare(string titlu, string mesaj)
        {
            var request = new NotificationRequest
            {
                NotificationId = 100,
                Title = titlu,
                Description = mesaj,
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1) 
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }


        private void OnCounterClicked(object sender, EventArgs e)
        {
            
            var request = new Plugin.LocalNotification.NotificationRequest
            {
                NotificationId = 1234,
                Title = "TEST",
                Description = "Salut! Dacă vezi asta, notificările funcționează!",
                Schedule = new Plugin.LocalNotification.NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };

            Plugin.LocalNotification.LocalNotificationCenter.Current.Show(request);

          
            DisplayAlert("Info", "Am trimis notificarea. Uită-te jos în dreapta!", "OK");
        }
    }
}