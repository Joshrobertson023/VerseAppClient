using DBAccessLibrary.Models;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VerseApp.Pages.Authentication
{
    public partial class Login : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        private string errorMessage;
        private string message;
        private string username;
        private string password;
        private int retryCount;
        private string overlayMessage;

        #region passwordTextField
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        void ButtonTestclick()
        {
            if(isShow)
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

        private async Task Signin_Click()
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    message = "Please enter a username";
                    return;
                }
                if (string.IsNullOrEmpty(password))
                {
                    message = "Please enter a password";
                    return;
                }

                overlayVisible = true;
                message = "";
                errorMessage = "";

                await dataservice.GetAllUsernames();

                data.usernames.Sort();
                username = username.Trim();
                int index = data.usernames.BinarySearch(username);

                if (index < 0)
                {
                    message = "Username does not exist.";
                    overlayVisible = false;
                    return;
                }

                string passwordHash = await dataservice.GetPasswordHash(username);
                if (!PasswordHash.VerifyHash(password.Trim(), passwordHash))
                {
                    message = "Incorrect password.";
                    return;
                }

                data.currentUser = new UserModel();
                data.currentUser = await dataservice.GetUser(username);
                if (data.currentUser == null)
                {
                    errorMessage = "There was an error logging you in.";
                    return;
                }
                await localStorage.SetItemAsync("authToken", data.currentUser.AuthToken);
                nav.NavigateTo("/");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (errorMessage.ToLower().Contains("timed out"))
                {
                    errorMessage = "Connection timed out.";
                    retryCount++;
                    overlayMessage = $"We had trouble connecting.\nRetrying ({retryCount})...";
                    await Signin_Click();
                }
                else
                {
                    errorMessage = "We encountered an error. Please try again. " + ex.Message;
                    overlayVisible = false;
                }
            }
        }

        private void CreateAnAccount_Click()
        {
            nav.NavigateTo("/authentication/createaccount");
        }
    }
}