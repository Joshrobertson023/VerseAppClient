using DBAccessLibrary;
using Microsoft.AspNetCore.Components;

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
        }

        private void GoToLogin()
        {
            nav.NavigateTo("authentication/login");
        }
    }
}
