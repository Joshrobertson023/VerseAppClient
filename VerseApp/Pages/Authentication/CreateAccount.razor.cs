using DBAccessLibrary.Models;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VerseApp.Pages.Authentication
{
    public partial class CreateAccount : ComponentBase
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
        private string username;
        private string? email;
        private int retryCount;
        private string overlayMessage;
        private int progress = 0;
        private bool NoEmail_Checkbox { get; set; }
        private string securityAnswer;
        private string customSecurityQuestion;
        private string securityQuestion;
        private string[] securityQuestions =
        {
            "What city were you born in?",
            "What is your oldest sibling's middle name?",
            "What was the first concert you attended?",
            "What was the make and model of your first car?",
            "What is your mother's maiden name?",
            "What was the name of your first pet?"
        };
        Random rand = new Random();

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

        private void GenerateUsername()
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                return;
            if (string.IsNullOrEmpty(username))
                username = firstName + lastName + rand.Next(100);
        }

        private async Task Continue_Click()
        {
            try
            {
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(username))
                {
                    message = "Please enter all required fields.";
                    return;
                }
                if (NoEmail_Checkbox)
                {
                    if (string.IsNullOrEmpty(securityQuestion))
                    {
                        message = "Please choose a security question.";
                        return;
                    }
                    if (string.IsNullOrEmpty(securityAnswer))
                    {
                        message = "Please choose a security answer.";
                        return;
                    }
                }

                progress = 0;
                overlayVisible = true;
                progress = 37;
                overlayMessage = "Checking usernames...";
                await Task.Delay(1);
                int exists = await dataservice.GetAllUsernames(username.Trim());
                progress = 78;
                overlayMessage = "Checking if your username is available...";
                await Task.Delay(1);

                if (exists == 1)
                {
                    message = "Username already exists.";
                    overlayVisible = false;
                    return;
                }
                else if (exists == 2)
                {
                    errorMessage = "There was an error connecting to the database. Please try again.";
                    overlayVisible = false;
                    return;
                }

                overlayVisible = false;
                if (string.IsNullOrEmpty(email))
                {
                    email = "NULL";
                    securityQuestion = securityQuestion.Substring(0, securityQuestion.Length - 1);
                }
                else
                {
                    securityQuestion = "NULL";
                    securityAnswer = "NULL";
                }
                progress = 100;
                overlayMessage = "Done!";
                await Task.Delay(1);
                nav.NavigateTo($"/authentication/createpassword/{firstName}/{lastName}/{username}/{email}");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (errorMessage.ToLower().Contains("timed out"))
                {
                    errorMessage = "Connection timed out.";
                    retryCount++;
                    overlayMessage = $"We had trouble connecting.\nRetrying ({retryCount})...";
                    await Continue_Click();
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

        private void Back3_Click()
        {
            nav.NavigateTo("/authentication/login");
        }
    }
}