using DBAccessLibrary.Models;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using System.Net.Mail;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VerseApp.Pages.Authentication.Forgot
{
    public partial class Password : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        private string errorMessage;
        private string username;
        private string message = string.Empty;
        private string? email;
        private string overlayMessage;
        private int progress = 0;
        private bool resetLinkSent = false;

        #region passwordTextField
        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        private IConfiguration _config;
        private string emailPassword;
        public Password(IConfiguration config)
        {
            _config = config;
            emailPassword = _config.GetConnectionString("Email");
        }

        public Password() { }

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
        private async Task Continue_Click()
        {
            //try
            //{
                if (string.IsNullOrWhiteSpace(email))
                {
                    message = "Please enter your email.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(username))
                {
                    message = "Please enter your username.";
                    return;
                }

                overlayVisible = true;
                progress = 23;
                overlayMessage = "Searching for your info...";
                await Task.Delay(200);
                message = "";
                errorMessage = "";
                await dataservice.GetRecoveryInfo();

                if (data.recoveryInfo == null || data.recoveryInfo.Count == 0)
                {
                    message = "No recovery records found.";
                    overlayVisible = false;
                    return;
                }
                string debug = "";
                progress = 84;
                overlayMessage = "Searching for your info...";
                await Task.Delay(200);
                foreach (var _recoveryInfo in data.recoveryInfo)
                {
                    debug = "";
                    if (_recoveryInfo.Username == username.Trim() && email == _recoveryInfo.Email)
                    {
                        progress = 100;
                        overlayMessage = "Done!";
                        await Task.Delay(200);
                        overlayVisible = false;
                        await dataservice.SendResetLink(username.Trim(), email.Trim());
                        message = "Check your inbox for the reset link.";
                        return;
                    }
                }
                message = "There is no-one in our records with that email and username.";
                progress = 0;
                overlayMessage = "";
                overlayVisible = false;
            //}
            //catch (Exception ex)
            //{
            //    errorMessage = ex.Message;
            //}
        }

        private void BackToLogin_Click()
        {
            nav.NavigateTo("/authentication/login");
        }
    }
}