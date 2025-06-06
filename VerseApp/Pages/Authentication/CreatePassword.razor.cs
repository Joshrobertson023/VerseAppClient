using DBAccessLibrary.Models;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.JSInterop;

namespace VerseApp.Pages.Authentication
{
    public partial class CreatePassword : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        [Parameter]
        public string username { get; set; }
        [Parameter]
        public string firstName { get; set; }
        [Parameter]
        public string lastName { get; set; }
        [Parameter]
        public string? email { get; set; }
        [Parameter]
        public string? securityQuestion { get; set; }
        [Parameter]
        public string? securityAnswer { get; set; }
        private string errorMessage;
        private string message;
        private string password;
        private string confirmPassword;
        private int retryCount;
        private string overlayMessage;
        private int progress = 0;

        #region passwordTextField
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        private async Task Back_Click()
        {
            nav.NavigateTo("/createaccount");
        }

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

        void ButtonTestclick2()
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

        private void Back2_Click()
        {
            nav.NavigateTo("/authentication/createaccount");
        }

        private async Task CreateAccount2_Click()
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    message = "Please enter a password.";
                    return;
                }
                if (string.IsNullOrEmpty(confirmPassword))
                {
                    message = "Please re-enter your password.";
                    return;
                }
                if (password.Trim() != confirmPassword.Trim())
                {
                    message = "Passwords do not match.";
                    return;
                }

                overlayVisible = true;
                progress = 13;
                await Task.Delay(100);

                if (email != "NULL")
                {
                    email = email.Trim();
                }
                else
                {
                    securityQuestion = securityQuestion.Trim();
                    securityAnswer = securityAnswer.Trim();
                }

                    Guid guid = Guid.NewGuid();
                string token = guid.ToString();
                progress = 89;
                StateHasChanged();
                await Task.Delay(200);
                string hashedPassword = PasswordHash.CreateHash(password.Trim());

                progress = 92;
                StateHasChanged();
                await Task.Delay(200);
                await dataservice.AddUserAsync(new UserModel(username.Trim(), 
                                                             firstName.Trim(), 
                                                             lastName.Trim(), 
                                                             email, 
                                                             securityQuestion, 
                                                             securityAnswer,
                                                             hashedPassword,
                                                             token));

                progress = 98;
                StateHasChanged();
                await Task.Delay(200);
                await localStorage.ClearAsync();
                await localStorage.SetItemAsync("authToken", token);
                progress = 100;
                await Task.Delay(10);
                overlayVisible = false;
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
                    await CreateAccount2_Click();
                }
                else
                {
                    errorMessage = "We encountered an error. Please try again. " + ex.Message;
                    overlayVisible = false;
                }
            }
        }

        private void DataPrivacy_Click()
        {
            nav.NavigateTo("/authentication/dataprivacy");
        }
    }
}