﻿using DBAccessLibrary;
using DBAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        private int progress;

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
        private List<string> httpLogs = new List<string>();

        protected override void OnInitialized()
        {
            dataservice.OnStatusChanged += HandleDataServiceStatus;
        }

        private void HandleDataServiceStatus(string message)
        {
            httpLogs.Add(message);
            StateHasChanged();
        }

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
                progress = 3;
                overlayMessage = "Connecting to server...";
                StateHasChanged();
                await Task.Delay(200);
                message = "";
                errorMessage = "";


                int result = await dataservice.CheckIfUsernameExists(username.Trim());

                if (result == 0)
                {
                    message = "Username does not exist.";
                    overlayVisible = false;
                    overlayMessage = "";
                    progress = 0;
                    return;
                }
                else if (result == 2)
                {
                    errorMessage = "There was an error connecting to the database. Please try again.";
                    overlayVisible = false;
                    return;
                }

                progress = 55;
                overlayMessage = "Checking your password...";
                StateHasChanged();
                await Task.Delay(200);
                string passwordHash = await dataservice.GetPasswordHash(username);
                if (!PasswordHash.VerifyHash(password.Trim(), passwordHash))
                {
                    message = "Incorrect password.";
                    overlayVisible = false;
                    overlayMessage = "";
                    progress = 0;
                    return;
                }

                progress = 89;
                overlayMessage = "Loading your data...";
                StateHasChanged();
                await Task.Delay(200);
                data.currentUser = new UserModel();
                data.currentUser = await dataservice.GetUser(username);
                // Get collections
                if (data.currentUser == null)
                {
                    errorMessage = "There was an error logging you in.";
                    return;
                }
                await localStorage.SetItemAsync("authToken", data.currentUser.AuthToken);
                progress = 100;
                overlayMessage = "Done!";
                await Task.Delay(1);
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

        private async Task CheckForEnterKey(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Signin_Click();
            }
        }

        private void ForgotPassword_Click()
        {
            nav.NavigateTo("/authentication/forgot/password");
        }

        private void ForgotUsername_Click()
        {
            nav.NavigateTo("/authentication/forgot/username");
        }
    }
}