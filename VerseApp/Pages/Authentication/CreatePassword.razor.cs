using DBAccessLibrary;
using DBAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections;

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
        [Inject]
        LoginInfo loginInfo { get; set; }
        [Parameter]
        public string username { get; set; }
        [Parameter]
        public string firstName { get; set; }
        [Parameter]
        public string lastName { get; set; }
        [Parameter]
        public string? email { get; set; }
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

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(loginInfo.Password))
                password = loginInfo.Password;

            if (!string.IsNullOrEmpty(loginInfo.RepeatPassword))
                confirmPassword = loginInfo.RepeatPassword;
        }

        private void CloseDrawer()
        {

        }

        private async Task CheckForEnterKey(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await CreateAccount2_Click();
            }
        }

        private void Back2_Click()
        {
            if (!string.IsNullOrEmpty(password))
                loginInfo.Password = password;
            if (!string.IsNullOrEmpty(confirmPassword))
                loginInfo.RepeatPassword = confirmPassword;
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

                    Guid guid = Guid.NewGuid();
                string token = guid.ToString();
                progress = 34;
                overlayMessage = "Securing your password...";
                StateHasChanged();
                await Task.Delay(200);
                string hashedPassword = PasswordHash.CreateHash(password.Trim());

                progress = 66;
                overlayMessage = "Pushing account to the server...";
                StateHasChanged();
                await Task.Delay(200);
                await dataservice.AddUserAsync(new UserModel(username.Trim(), 
                                                             firstName.Trim(), 
                                                             lastName.Trim(), 
                                                             email,
                                                             hashedPassword,
                                                             token));

                data.currentUser = await dataservice.GetUser(username.Trim());
                progress = 71;
                StateHasChanged();
                await Task.Delay(200);

                Collection newCollection = new Collection();
                newCollection.Title = "Favorites";
                newCollection.UserVerses = new List<UserVerse>();
                newCollection.Author = username.Trim();
                newCollection.NumVerses = 0;
                newCollection.Visibility = 2;
                newCollection.UserId = data.currentUser.Id;
                newCollection.Pinned = 1;
                newCollection.VerseOrder = "none";
                await dataservice.AddNewCollection(newCollection);
                progress = 78;
                StateHasChanged();
                await Task.Delay(200);

                progress = 89;
                overlayMessage = "Saving your local signin...";
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
            if (!string.IsNullOrEmpty(password))
                loginInfo.Password = password;
            if (!string.IsNullOrEmpty(confirmPassword))
                loginInfo.RepeatPassword = confirmPassword;
            nav.NavigateTo("/authentication/dataprivacy");
        }
    }
}