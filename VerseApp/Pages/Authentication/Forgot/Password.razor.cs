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
        private string message;
        private string username;
        private string? email;
        private string overlayMessage;
        private int progress = 0;
        private bool NoEmail_Checkbox { get; set; }
        private string securityAnswer;
        private string securityQuestion;
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
        private async Task GetSecurityQuestion_Click()
        {
            securityQuestion = await dataservice.GetSecurityQuestion(username.Trim());
        }

        private async Task Continue_Click()
        {
            try
            {
                RecoveryInfo userRecovering = new RecoveryInfo();
                userRecovering.Username = username.Trim();
                var _email = new MailAddress(email);
                string token = Guid.NewGuid().ToString();
                string resetUrl = $"https://localhost:7093/resetpassword?token={token}&username={userRecovering.Username}";

                var sendingAddress = new MailAddress("therealjoshrobertson@gmail.com");
                string subject = "Reset Your Password";
                string body = $"Click the link below to reset your password:\n\n{resetUrl}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("therealjoshrobertson@gmail.com", "tofp kaki lkuv nffh") // Change emailPassword for prod
                };

                using var message = new MailMessage(sendingAddress, _email)
                {
                    Subject = subject,
                    Body = body
                };

                await smtp.SendMailAsync(message);
                await dataservice.PutResetToken(username, token);
                resetLinkSent = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}