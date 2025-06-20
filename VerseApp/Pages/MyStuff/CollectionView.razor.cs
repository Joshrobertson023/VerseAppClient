using DBAccessLibrary;
using DBAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace VerseApp.Pages.MyStuff
{
    public partial class CollectionView : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        private IDialogService dialogService { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        public bool addingVerse { get; set; }
        private bool loading { get; set; }
        private int progress;
        private string overlayMessage { get; set; }
        private string errorMessage { get; set; }
        private bool reorganizing = false;
        private bool overlayVisible = false;
        [Parameter]
        public string _collectionId { get; set; } = string.Empty;
        Collection currentCollection { get; set; }
        private string message { get; set; }

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }


        bool otherUserCollection = false;
        bool authorized = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                loading = true;
                int collectionId = Convert.ToInt32(_collectionId);

                Collection pageCollection = data.currentUser.Collections.FirstOrDefault(c => c.Id == collectionId);

                bool needsLoad = pageCollection.UserVerses == null || !pageCollection.UserVerses.Any()
                    || pageCollection.UserVerses.All(uv => uv.Verses == null || !uv.Verses.Any());

                if (needsLoad)
                {
                    Collection loaded = await dataservice.GetVersesByCollectionAsync(pageCollection);
                    pageCollection.UserVerses = loaded.UserVerses;
                }

                // If the collection's author is not the current user:
                otherUserCollection = pageCollection.UserId != data.currentUser.Id;
                authorized = pageCollection.Visibility == 2 || (pageCollection.Visibility == 0 && !otherUserCollection);
                // Implement if friends collection once friend logic is implemented

                currentCollection = pageCollection;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading collection: {ex.Message}";
                if (ex.Message.Contains("Sequence contains no elements"))
                {
                    loading = false;
                    errorMessage = "";
                    message = "No passages.";
                }
            }
            finally
            {
                loading = false;
            }
        }

        private void Expand(UserVerse userVerse)
        {
            data.collapsedBefore = true;
            userVerse.Expanded = !userVerse.Expanded;
        }

        private void AddPassage()
        {

        }

        private async Task Back_Click()
        {
            await JSRuntime.InvokeVoidAsync("history.back");
        }

        private void CloseDrawer()
        {

        }

        private bool open;
        private void ToggleSettingsOpen()
        {
            open = !open;
        }

        private void AddCollection() // User gets max of 50 collections, with 100 verses max each
        {
            nav.NavigateTo("/mystuff/newcollection");
        }

        private void EditCollection(Collection collection)
        {

        }

        private void ShareCollection(Collection collection)
        {

        }

        private void PublishCollection(Collection collection)
        {

        }

        private async Task DeleteCollection(Collection collection)
        {
            try
            {
                bool? result = await dialogService.ShowMessageBox(
                    "Are you sure you want to delete this collection?",
                    " ",
                    yesText: "Delete", cancelText: "Cancel");
                if (result != null)
                {
                    data.currentUser.Collections.Remove(collection);
                    await dataservice.DeleteCollectionAsync(collection.Id);
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting collection: {ex.Message}";
            }
        }
    }
}
