using DBAccessLibrary;
using DBAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace VerseApp.Pages.MyStuff
{
    public partial class Collections
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        private IDialogService dialogService { get; set; }
        public bool addingVerse { get; set; }
        private bool loading { get; set; }
        private int progress;
        private string overlayMessage { get; set; }
        private string errorMessage { get; set; }
        private bool reorganizing = false;
        private bool overlayVisible = false;
        private int sortBy = 0; // 0 = Title, 1 = Date Created, 2 = Date Practiced, 3 = Most Completed, 4 = Custom

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }

        private void OpenCollection(Collection collection)
        {
            if (collection != null)
            {
                nav.NavigateTo($"/mystuff/collection/{collection.Id}");
            }
        }

        private bool isLoading = true;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                while (data.currentUser.Id <= 0)
                {
                    await Task.Delay(100);
                    Console.WriteLine("Waiting to get collections");
                }

                data.currentUser.Collections = await dataservice.GetUserCollections(data.currentUser.Id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
            }
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
                    "Are you sure you want to delete this collection and all passages in it? This cannot be undone.",
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
