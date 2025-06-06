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
                if (string.IsNullOrWhiteSpace(password))
                {
                    message = "Please enter your password.";
                    return;
                }

                overlayVisible = true;
                progress = 3;
                overlayMessage = "Getting info from database...";
                await Task.Delay(200);
                message = "";
                errorMessage = "";
                await dataservice.GetRecoveryInfo();

                string recoveryFullName = firstName.Trim().ToLower() + lastName.Trim().ToLower();
                string hashedRecoveryPassword = PasswordHash.CreateHash(password.Trim());
                string debug = "";
                progress = 84;
                overlayMessage = "Searching for your info...";
                await Task.Delay(200);
                foreach (var _recoveryInfo in data.recoveryInfo)
                {
                    debug = "";
                    if (_recoveryInfo.FirstName + _recoveryInfo.LastName == recoveryFullName && PasswordHash.VerifyHash(password.Trim(), _recoveryInfo.PasswordHash))
                    {
                        progress = 100;
                        overlayMessage = "Done!";
                        await Task.Delay(200);
                        overlayVisible = false;
                        message = "Your username is " + _recoveryInfo.Username;
                        return;
                    }
                    debug += _recoveryInfo.FullName + " | " + recoveryFullName + " Password check: " + PasswordHash.VerifyHash(password.Trim(), _recoveryInfo.PasswordHash).ToString();
                }

                progress = 100;
                await Task.Delay(200);
                overlayMessage = "Done!";
                message = "Your name or password is incorrect." + " _recoveryInfo.FullName: " + debug;
                overlayVisible = false;
            }
            catch (Exception ex)
            {
                overlayVisible = false;
                errorMessage = "We encountered an error. Please try again. " + ex.Message;
            }
        }

        private async Task ForgotPasswordUsername_Click()
        {
            //            //try
            //            //{
            //            //    if (string.IsNullOrWhiteSpace(firstName))
            //            //        throw new Exception("Please enter your first name.");
            //            //    if (string.IsNullOrWhiteSpace(lastName))
            //            //        throw new Exception("Please enter your last name.");
            //            //    if (string.IsNullOrWhiteSpace(recoveryEmail))
            //            //        throw new Exception("Please enter an email.");

            //            //    loading = true;
            //            //    message = "";
            //            //    errorMessage = "";
            //            //    List<RecoveryInfo> recoveryInfo = await userservice.GetRecoveryInfoDBAsync();

            //            //    string recoveryFullName = firstName.Trim().ToLower() + lastName.Trim().ToLower();

            //            //    foreach (var _recoveryInfo in recoveryInfo)
            //            //    {
            //            //        if (_recoveryInfo.FirstName + _recoveryInfo.LastName == recoveryFullName && PasswordHash.VerifyHash(recoveryEmail.Trim(), _recoveryInfo.Email))
            //            //        {
            //            //            await SendResetLink(_recoveryInfo);
            //            //            loading = false;
            //            //            return;
            //            //        }
            //            //    }

            //            //    message = "That email does not match the one on file with your name.";
            //            //    enteringName = true;
            //            //    loading = false;
            //            //}
            //            //catch (Exception ex)
            //            //{
            //            //    errorMessage = ex.Message;
            //            //}
        }

        private void CreateAnAccount_Click()
        {
            nav.NavigateTo("/authentication/createaccount");
        }
    }
}