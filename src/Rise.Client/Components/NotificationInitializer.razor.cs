using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Rise.Shared.Notifications;

namespace Rise.Client.Components;

/// <summary>
/// Component that manages push notification initialization and permission prompts.
/// Handles browser subscription creation and server synchronization.
/// </summary>
public partial class NotificationInitializer
{
    /// <summary>JavaScript runtime for interop.</summary>
    [Inject]
    public IJSRuntime JS { get; set; } = default!;
    
    /// <summary>Service for notification preferences.</summary>
    [Inject]
    public required INotificationPreferencesService NotificationPreferencesService { get; set; }
    
    /// <summary>VAPID public key for web push notifications.</summary>
    [Parameter]
    public string VapIdPublicKey { get; set; } = "BCW-qlnpFfIjUDSN5cg0JUah1ktLevpGuU0HgBLvj76rpPinTndjtmEjZriWPsooBzKIJ4oEsTs8c1yAyCHBwGI";

    /// <summary>Whether to show the notification permission prompt.</summary>
    public bool Show { get; set; } = false;

    /// <summary>
    /// Checks notification state on first render.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Console.WriteLine("NotificationInitializer: First render, checking notification state...");
            await CheckStateAsync();
        }
    }

    /// <summary>
    /// Checks the notification permission state and shows prompt if needed.
    /// </summary>
    private async Task CheckStateAsync()
    {
        var permission = await JS.InvokeAsync<string>("eval", "Notification.permission");
        Console.WriteLine($"NotificationInitializer: Notification permission is '{permission}'");

        if (permission == "default")
        {
            // Check browser subscription
            var exists = await JS.InvokeAsync<bool>("checkExistingSubscription");
            Console.WriteLine($"NotificationInitializer: Existing subscription check: {exists}");
            if (!exists)
            {
                Console.WriteLine("NotificationInitializer: Showing notification prompt");
                Show = true;
                StateHasChanged();
            }
            else
            {
                Console.WriteLine("NotificationInitializer: Subscription already exists, not showing prompt");
            }
        }
        else
        {
            Console.WriteLine($"NotificationInitializer: Permission is '{permission}', not showing prompt");
        }
    }

    /// <summary>
    /// Handles user denial of notification permissions.
    /// </summary>
    private Task Deny()
    {
        Console.WriteLine("NotificationInitializer: User denied notifications");
        Show = false;
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles user acceptance of notification permissions and creates subscription.
    /// </summary>
    private async Task Allow()
    {
        try
        {
            Console.WriteLine("NotificationInitializer: User allowed notifications, subscribing...");

            var subscribeResult = await NotificationPreferencesService.Subscribe(new PushSubscriptionRequest.Create
            {
                Endpoint = string.Empty,
                Keys = new PushSubscriptionRequest.Keys
                {
                    P256dh = string.Empty,
                    Auth = string.Empty
                }
            }, CancellationToken.None);

            if (subscribeResult.IsSuccess)
            {
                Console.WriteLine("NotificationInitializer: Subscription successful, updating preferences...");

                Show = false;
                StateHasChanged();
            }
            else
            {
                Console.WriteLine($"NotificationInitializer: Error subscribing to notifications: {subscribeResult.Errors.FirstOrDefault()}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"NotificationInitializer: Error during notification subscription: {ex.Message}");
            Console.WriteLine(ex);
        }
    }
}