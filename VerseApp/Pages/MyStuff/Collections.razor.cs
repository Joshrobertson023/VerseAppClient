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
        [Inject]
        CurrentPageStructure pageStructure { get; set; }
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
                pageStructure.MyStuff = $"/mystuff/collection/{collection.Id}";
                nav.NavigateTo($"/mystuff/collection/{collection.Id}");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // now that the Razor markup (and your #scrollTopBtn) is in the DOM,
                // wire up the scroll + click handlers
                await JSRuntime.InvokeVoidAsync("scrollToTopInit");
            }
        }

        private bool isLoading = true;
        protected override async Task OnInitializedAsync()
        {
            pageStructure.MyStuff = "/";
            try
            {
                while (data.currentUser.Id <= 0)
                {
                    await Task.Delay(100);
                    Console.WriteLine("Waiting to get collections");
                }

                if (data.currentUser.Collections == null || data.currentUser.Collections.Count == 0)
                    data.currentUser.Collections = await dataservice.GetUserCollections(data.currentUser.Id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                isLoading = false;
                Console.WriteLine("Collections loaded.");
                foreach (var collection in data.currentUser.Collections)
                    Console.WriteLine($"Collection: {collection.Title}, ID: {collection.Id}");
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
            pageStructure.MyStuff = "/mystuff/newcollection";
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
