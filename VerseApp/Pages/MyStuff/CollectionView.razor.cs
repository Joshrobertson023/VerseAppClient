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

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }


        protected override async Task OnInitializedAsync()
        {
            try
            {
                int collectionId = Convert.ToInt32(_collectionId);

                Collection existingCollection = data.currentUser.Collections.FirstOrDefault(c => c.Id == collectionId);

                if (existingCollection != null)
                {
                    currentCollection = existingCollection;
                }
                else
                {
                    currentCollection = await dataservice.GetCollectionAsync(collectionId);
                    if (currentCollection != null)
                    data.currentUser.Collections.Add(currentCollection);
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading collection: {ex.Message}";
            }
        }

        private void Expand(UserVerse userVerse)
        {
            userVerse.Expanded = !userVerse.Expanded;
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
