using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotification;

namespace ApartmentManager.Maui.Services;

public class NotificationClientService(HubConnection? hubConnection = null)
{
    private const string BaseUrl = "http://localhost:5191/notificationHub";
    private readonly HashSet<int> _seenNotificationIds = new();
    private HubConnection? _hubConnection = hubConnection;

    public async Task InitializeAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        if (string.IsNullOrEmpty(token)) return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(BaseUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                options.HttpMessageHandlerFactory = handler =>
                {
                    if (handler is HttpClientHandler clientHandler)
                        clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    return handler;
                };
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string>("ReceiveNotification", (title, message) =>
        {
            MainThread.BeginInvokeOnMainThread(async void () =>
            {
                var request = new NotificationRequest
                {
                    NotificationId = new Random().Next(),
                    Title = title,
                    Description = message,
                    BadgeNumber = 1,
                    Schedule = { NotifyTime = DateTime.Now.AddSeconds(1) }
                };
                await LocalNotificationCenter.Current.Show(request);
            });
        });

        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR Connection Error: {ex.Message}");
        }

        StartPolling();
    }

    private void StartPolling()
    {
        Application.Current?.Dispatcher.DispatchDelayed(TimeSpan.FromMinutes(15), async void () =>
        {
            await CheckPendingNotifications();
            StartPolling();
        });
    }

    private async Task CheckPendingNotifications()
    {
        try
        {
            var notifications = await App.RestService.GetNotificationsAsync();
            foreach (var notif in notifications)
                if (!_seenNotificationIds.Contains(notif.Id))
                {
                    _seenNotificationIds.Add(notif.Id);

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        var request = new NotificationRequest
                        {
                            NotificationId = notif.Id,
                            Title = notif.Title,
                            Description = notif.Message,
                            BadgeNumber = 1,
                            Schedule = { NotifyTime = DateTime.Now.AddSeconds(1) }
                        };
                        await LocalNotificationCenter.Current.Show(request);
                    });
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Polling Error: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
}