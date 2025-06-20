using DBAccessLibrary;
using DBAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        public int SortBy { get => sortBy;
            set
            {
                sortBy = value;
                Console.WriteLine("SortBy: " + SortBy);
                data.currentUser.CollectionsSort = value;
                SortByChanged();
            }
        }
        //private int[] sortOptions = new int[] { 0, 1, 2, 3, 4 };
        private Dictionary<int, string> sortOptions = new Dictionary<int, string>
        {
            [0] = "Newest",
            [1] = "Title",
            [2] = "Last Practiced",
            [3] = "Completed",
            [4] = "Custom"
        };

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

        private async Task PinCollection(Collection collection)
        {
            collection.Pinned = collection.Pinned == 1 ? 0 : 1;

            //data.currentUser.CollectionsOrder = Algorithms.GetSortOrder(data.currentUser.Collections, 
            //                                                            data.currentUser.CollectionsSort);
            //Console.WriteLine($"CollectionsOrder: {data.currentUser.CollectionsOrder}");
            //data.currentUser.Collections = Algorithms.Sort(data.currentUser.Collections,
            //                                               data.currentUser.CollectionsOrder);
            
            await dataservice.TogglePinCollection(collection);
        }
        private int placeholderIndex = -1;

        void ClearPlaceholder()
        {
            placeholderIndex = -1;
        }

        void OnDragOver(int index, int pinnedCount)
        {
            if (draggingModel?.Collection.Pinned == 1)
                placeholderIndex = Math.Min(index, pinnedCount - 1);
            else
                placeholderIndex = Math.Max(index, pinnedCount);
        }

        void HandleDrop(ReorderModel landingModel, int pinnedCount)
        {
            if (draggingModel is null) return;

            int targetOrder = landingModel.Order;
            if (draggingModel.Collection.Pinned == 1)
                targetOrder = Math.Min(targetOrder, pinnedCount - 1);
            else
                targetOrder = Math.Max(targetOrder, pinnedCount);

            foreach (var m in reorderList.Where(x => x.Order >= targetOrder))
                m.Order++;
            draggingModel.Order = targetOrder;

            int idx = 0;
            foreach (var m in reorderList.OrderBy(x => x.Order))
            {
                m.Order = idx++;
                m.IsDragOver = false;
            }
        }

        private async Task Done_Click()
        {
            string newOrder = string.Empty;
            if (SortBy == 4)
            {
                newOrder = string.Join(",", reorderList.OrderBy(x => x.Order).Select(x => x.Collection.Id)) + ",";
                data.currentUser.CollectionsOrder = newOrder;
                data.currentUser.Collections = reorderList.OrderBy(x => x.Order).Select(x => x.Collection).ToList();
                await dataservice.UpdateCollectionsOrder(new OrderInfo
                {
                    UserId = data.currentUser.Id,
                    OrderBy = data.currentUser.CollectionsSort,
                    Order = newOrder
                });
            }
            else
            {
                data.currentUser.Collections = reorderList.OrderBy(x => x.Order).Select(x => x.Collection).ToList();
                await dataservice.UpdateCollectionsOrder(new OrderInfo
                {
                    UserId = data.currentUser.Id,
                    OrderBy = data.currentUser.CollectionsSort,
                    Order = data.currentUser.CollectionsOrder
                });
            }
            
            reorganizing = false;
        }

        private async Task SortByChanged()
        {
            if (!checkEnabled) return;
            checkEnabled = false;
            Sort();
            await dataservice.UpdateCollectionsOrder(new OrderInfo { UserId = data.currentUser.Id,
                                                                     OrderBy = data.currentUser.CollectionsSort, 
                                                                     Order = data.currentUser.CollectionsOrder });
            checkEnabled = true;
        }

        private bool checkEnabled = true;
        private void Sort()
        {
            if (SortBy == 4)
                (data.currentUser.CollectionsOrder, 
                 data.currentUser.Collections)      = Algorithms.SortCustom(data.currentUser.Collections,
                                                                              data.currentUser.CollectionsSort);
            Console.WriteLine($"CollectionsOrder: {data.currentUser.CollectionsOrder}");
            Console.WriteLine("Order by: " + data.currentUser.CollectionsSort);
            data.currentUser.Collections = Algorithms.Sort(data.currentUser.Collections,
                                                           data.currentUser.CollectionsSort);
        }

        private void Reorder_Click()
        {
            reorganizing = true;
            reorderList = data.currentUser.Collections.Select((c, i) => new ReorderModel
            {
                Order = i,
                Collection = c
            }).ToList();
        }

        private class ReorderModel
        {
            public int Order { get; set; }
            public Collection Collection { get; set; } = null!;
            public bool IsDragOver { get; set; }
        }

        private List<ReorderModel> reorderList = new();
        private ReorderModel? draggingModel;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
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
                SortBy = data.currentUser.CollectionsSort;
                Sort();
                foreach (var collection in data.currentUser.Collections)
                {
                    Console.WriteLine($"Collection: {collection.Title}, Pinned: {collection.Pinned}, Progress: {collection.ProgressPercent}");
                }
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
