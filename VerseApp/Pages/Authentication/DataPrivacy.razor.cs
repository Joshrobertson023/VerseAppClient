using DBAccessLibrary.Models;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel;
using Microsoft.JSInterop;

namespace VerseApp.Pages.Authentication
{
    public partial class DataPrivacy : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
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

        private async Task Back_Click()
        {
            await JSRuntime.InvokeVoidAsync("window.back");
        }

        private bool overlayVisible = false;

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }

        private void CloseDrawer()
        {

        }

        private void Continue_Click()
        {

        }

        private void DataPrivacy_Click()
        {

        }
    }
}