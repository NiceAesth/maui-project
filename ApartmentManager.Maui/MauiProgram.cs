using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace ApartmentManager.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwesome-Solid.otf", "FAsolid");
                fonts.AddFont("FontAwesome-Regular.otf", "FAregular");
                fonts.AddFont("FontAwesome-Brands.otf", "FAbrands");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}