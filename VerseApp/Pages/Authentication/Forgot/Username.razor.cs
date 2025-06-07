using DBAccessLibrary.Models;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VerseApp.Pages.Authentication.Forgot
{
    public partial class Username : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        private string errorMessage;
        private string message;
        private string firstName;
        private string lastName;
        private string email;
        private int retryCount;
        private string overlayMessage;
        private int progress;

        #region passwordTextField
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        void ButtonTestclick()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }
        #endregion

        private bool overlayVisible = false;

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }

        private void CloseDrawer()
        {

        }

        private async Task RecoverUsername_Click()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    message = "Please enter your first name.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    message = "Please enter your last name.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(email))
                {
                    message = "Please enter your email.";
                    return;
                }

                overlayVisible = true;
                progress = 23;
                overlayMessage = "Searching for your info...";
                await Task.Delay(200);
                message = "";
                errorMessage = "";
                await dataservice.GetRecoveryInfo();

                string recoveryFullName = firstName.Trim().ToLower() + lastName.Trim().ToLower();

                string debug = "";
                progress = 84;
                overlayMessage = "Searching for your info...";
                await Task.Delay(200);
                List<string> usernames = new List<string>();
                foreach (var _recoveryInfo in data.recoveryInfo)
                {
                    debug = "";
                    if (_recoveryInfo.FirstName + _recoveryInfo.LastName == recoveryFullName && email == _recoveryInfo.Email)
                    {
                        progress = 100;
                        overlayMessage = "Done!";
                        await Task.Delay(200);
                        overlayVisible = false;
                        usernames.Add(_recoveryInfo.Username);
                    }
                }

                if (usernames.Count == 0)
                {
                    progress = 100;
                    await Task.Delay(200);
                    overlayMessage = "Done!";
                    message = "The email does not match that name on our files. " + debug;
                    overlayVisible = false;
                    return;
                }
                else if (usernames.Count == 1)
                {
                    message = $"Your username is {usernames[0]}";
                    overlayVisible = false;
                    return;
                }
                else
                {
                    message = $"Usernames found with that info: {string.Join(", ", usernames)}";
                }
            }
            catch (Exception ex)
            {
                overlayVisible = false;
                errorMessage = "We encountered an error. Please try again. " + ex.Message;
            }
        }

        private void BackToLogin_Click()
        {
            nav.NavigateTo("/authentication/login");
        }
    }
}