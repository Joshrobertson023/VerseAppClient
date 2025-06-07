using DBAccessLibrary;
using Microsoft.AspNetCore.Components;

namespace VerseApp.Pages
{
    public partial class Search : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        [Inject]
        Data data { get; set; }
        public bool addingVerse { get; set; }
        private bool loading { get; set; }
        private int progress;
        private string overlayMessage { get; set; }
        private string errorMessage { get; set; }

        private bool overlayVisible = false;

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }

        private void CloseDrawer()
        {

        }

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

        private void Signin_Click()
        {
            nav.NavigateTo("authentication/login");
        }
    }
}
