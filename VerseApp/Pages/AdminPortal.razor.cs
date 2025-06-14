using DBAccessLibrary;
using DBAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VerseApp.Pages
{
    public partial class AdminPortal : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        private IDialogService dialogService { get; set; }
        private string overlayMessage { get; set; }
        private string errorMessage { get; set; }

        private bool overlayVisible = false;
        private string message;
        private int progress { get; set; } = 0;

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }

        private void CloseDrawer()
        {

        }

        private string notificationTitle;
        private string notificationBody;
        private string notificationType;
        private string notificationMessage { get; set; }

        private async Task SendGlobalNotification()
        {
            try
            {
                if (string.IsNullOrEmpty(notificationTitle) || string.IsNullOrEmpty(notificationBody) || string.IsNullOrEmpty(notificationType))
                {
                    errorMessage = "All fields are required to send a notification.";
                    return;
                }

                message = "Sending...";
                await Task.Delay(200);
                StateHasChanged();

                List<string> usernames = await dataservice.GetAllUsernamesAsync();

                if (notificationType == "App Notification")
                {
                    foreach (var username in usernames)
                    {
                        string notificationTitleWithUsername = notificationTitle.Replace("<username>", username);
                        string notificationBodyWithUsername = notificationBody.Replace("<username>", username);

                        var notification = new Notification
                        {
                            Title = notificationTitleWithUsername,
                            Message = notificationBodyWithUsername,
                            SentBy = "VerseApp Team",
                            Username = username
                        };
                        await dataservice.SendNotification(notification);
                    }
                }

                var emailnotification = new Notification
                {
                    Title = notificationTitle,
                    Message = notificationBody,
                    SentBy = "VerseApp Team",
                    Username = "everyone"
                };
                if (notificationType == "Email Notification")
                    await dataservice.SendEmailNotification(emailnotification);

                message = "Notification sent successfully.";
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred while sending the notification: {ex.Message}";
            }
        }
    }
}
