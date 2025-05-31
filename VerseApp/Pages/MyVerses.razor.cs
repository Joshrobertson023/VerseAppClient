using DBAccessLibrary;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace VerseApp.Pages
{
    public partial class MyVerses : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        DataService data { get; set; }
        public bool addingVerse { get; set; }
        private bool loading { get; set; }

        //List<Verse> versesInCategory = new List<Verse>();

        private void CategorizeVerses()
        {

        }

        private void ToggleAddVerses()
        {
            addingVerse = !addingVerse;
        }

        private async Task RefreshVerses()
        {
            addingVerse = false;
            loading = true;
            //await data.GetUserVersesDBAsync(data.currentUser.Id);
            //await data.GetUserCategoriesDBAsync(data.currentUser.Id);
            loading = false;
        }


        private string userName;
        private string email;

        protected override async Task OnInitializedAsync()
        {
            // Try to read data only if the user is authenticated.
            // But if you wrap all of this inside <Authorized> above, 
            // this code will run only after the user is signed in:
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                // e.g. populate `userName`, `email` by calling your API
                userName = user.Identity.Name;
                // or call an endpoint: user details come from your own backend
                // ...
            }
        }

        private void GoToLogin()
        {
            // This will redirect to your built-in Identity login page
            // (AddApiAuthorization wires /authentication/login by default)
            nav.NavigateTo("authentication/login");
        }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    }
}
