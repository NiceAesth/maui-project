using Plugin.LocalNotification;
using PROIECT.Data;
using PROIECT.Services;
using PROIECT.Models;
using System.IO;

namespace PROIECT
{
    public partial class App : Application
    {
        static Database? database;
        static RestService? restService;
        public static Locatar UserLogat { get; set; }
        public static Database Database
        {
            get
            {
                if (database == null)
                    
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PROIECT.db3"));
                return database;
            }
        }

        public static RestService RestService
        {
            get
            {
                if (restService == null)
                    restService = new RestService();
                return restService;
            }
        }

        public App()
        {
            InitializeComponent();
           

            int loggedId = Preferences.Get("LoggedLocatarId", 0);
            if (loggedId > 0)
            {
                MainPage = new AppShell();
                Task.Run(async () =>
                {
                    UserLogat = await Database.GetLocatarAsync(loggedId);
                });
            }
        
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        

        public static void TrimiteNotificare(string titlu, string mesaj)
        {
            var request = new NotificationRequest
            {
                NotificationId = 1000,
                Title = titlu,
                Description = mesaj,
                BadgeNumber = 1,
                Schedule = { NotifyTime = DateTime.Now.AddSeconds(1) }
            };
            LocalNotificationCenter.Current.Show(request);
        }
    }
}