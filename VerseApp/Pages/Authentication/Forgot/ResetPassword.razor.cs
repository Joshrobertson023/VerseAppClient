﻿using DBAccessLibrary.Models;
using DBAccessLibrary;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VerseApp.Pages.Authentication.Forgot
{
    public partial class ResetPassword : ComponentBase
    {
        [Parameter]
        public string token { get; set; }
        [Parameter]
        public string username { get; set; }
        [Inject]
        DataService dataservice { get; set; }

        [Inject]
        private NavigationManager Nav { get; set; }
        private string errorMessage;
        private string password;
        private string repeatPassword;
        private string? email;
        private string overlayMessage;
        private int progress = 0;
        private bool NoEmail_Checkbox { get; set; }
        private string securityAnswer;
        private string securityQuestion;
        private bool resetLinkSent = false;
        private bool validToken = false;
        private string message;

        #region passwordTextField
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        protected override async Task OnInitializedAsync()
        {
            validToken = await dataservice.VerifyTokenDBAsync(username, token);
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

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                validToken = await dataservice.VerifyTokenDBAsync(username, token);
            }
            catch (Exception ex)
            {
                errorMessage = "Unable to verify reset link. Please try again later.";
            }
        }

        private async Task ResetPassword_Click()
        {
            try
            { 
                if (password.Trim() != repeatPassword.Trim())
                {
                    message = "Passwords do not match.";
                    return;
                }

                overlayVisible = true;
                progress = 67;
                await Task.Delay(200);
                int userId = await dataservice.GetIdFromUsername(username.Trim());
                progress = 89;
                await Task.Delay(200);
                await dataservice.UpdateUserPassword(userId, PasswordHash.CreateHash(password.Trim()));
                overlayVisible = false;
                message = "Go back to the original window to sign in.";
            }
            catch (Exception ex)
            {
                errorMessage = "We ran into an error. Please try again.";
            }
        }
    }
}