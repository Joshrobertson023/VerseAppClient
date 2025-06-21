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
        List<Collection> collections = new List<Collection>();
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
            Sort();
            await dataservice.TogglePinCollection(collection);
        }

        string pinnedOrder = string.Empty;
        private void BuildPinned()
        {
            pinnedOrder = string.Empty;
            foreach (var collection in data.currentUser.Collections.Where(c => c.Pinned == 1))
            {
                pinnedOrder += $"{collection.Id},";
            }
            Console.WriteLine("PinnedOrder: " + pinnedOrder);
            if (string.IsNullOrEmpty(pinnedOrder))
                pinnedOrder = "NONE";
        }

        private int placeholderIndex = -1;

        void ClearPlaceholder()
        {
            placeholderIndex = -1;
        }

        void OnDragOver(int index, int pinnedCount)
        {
            //if (draggingModel?.Collection.Pinned == 1)
            //    placeholderIndex = Math.Min(index, pinnedCount - 1);
            //else
            //    placeholderIndex = Math.Max(index, pinnedCount);
        }

        void HandleDrop(ReorderModel landingModel, int pinnedCount)
        {
            //if (draggingModel is null) return;

            //int targetOrder = landingModel.Order;
            //if (draggingModel.Collection.Pinned == 1)
            //    targetOrder = Math.Min(targetOrder, pinnedCount - 1);
            //else
            //    targetOrder = Math.Max(targetOrder, pinnedCount);

            //foreach (var m in reorderList.Where(x => x.Order >= targetOrder))
            //    m.Order++;
            //draggingModel.Order = targetOrder;

            //int idx = 0;
            //foreach (var m in reorderList.OrderBy(x => x.Order))
            //{
            //    m.Order = idx++;
            //    m.IsDragOver = false;
            //}
        }

        private async Task Done_Click()
        {
            string newOrder = string.Empty;
            //if (SortBy == 4)
            //{
            //    newOrder = string.Join(",", reorderList.OrderBy(x => x.Order).Select(x => x.Collection.Id)) + ",";
            //    data.currentUser.CollectionsOrder = newOrder;
            //    data.currentUser.Collections = reorderList.OrderBy(x => x.Order).Select(x => x.Collection).ToList();
            //    await dataservice.UpdateCollectionsOrder(new OrderInfo
            //    {
            //        UserId = data.currentUser.Id,
            //        OrderBy = data.currentUser.CollectionsSort,
            //        Order = newOrder
            //    });
            //}
            //else
            //{
            //    data.currentUser.Collections = reorderList.OrderBy(x => x.Order).Select(x => x.Collection).ToList();
            //    await dataservice.UpdateCollectionsOrder(new OrderInfo
            //    {
            //        UserId = data.currentUser.Id,
            //        OrderBy = data.currentUser.CollectionsSort,
            //        Order = data.currentUser.CollectionsOrder
            //    });
            //}
            
            reorganizing = false;
        }

        private async Task SortByChanged()
        {
            if (!checkEnabled) return;
            checkEnabled = false;
            Sort();
            Console.WriteLine("SortByChanged called with SortBy: " + SortBy);
            await dataservice.UpdateCollectionsOrder(new OrderInfo
            {
                UserId = data.currentUser.Id,
                OrderBy = data.currentUser.CollectionsSort,
                Order = data.currentUser.CollectionsOrder
            });
            checkEnabled = true;
        }

        private bool checkEnabled = true;
        private void Sort()
        {
            BuildPinned();
            if (SortBy == 4)
                (data.currentUser.Collections, data.currentUser.CollectionsOrder, pinnedOrder)
                    = Algorithms.SortCustom(data.currentUser.Collections, data.currentUser.CollectionsOrder, pinnedOrder);
            else
                data.currentUser.Collections = Algorithms.Sort(data.currentUser.Collections, SortBy, pinnedOrder);

            data.currentUser.CollectionsSort = SortBy;
            Console.WriteLine($"\nCollectionsOrder (unchanged if not 4): {data.currentUser.CollectionsOrder}");
        }

        private void Reorder_Click()
        {
        //    reorganizing = true;
        //    reorderList = data.currentUser.Collections.Select((c, i) => new ReorderModel
        //    {
        //        Order = i,
        //        Collection = c
        //    }).ToList();
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
        private string customOrderBackup;
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

                while (data.currentUser.Id == null || data.currentUser.Id <= 0)
                {
                    await Task.Delay(2000);
                    Console.WriteLine("Waiting to grab collections, userId is null.");
                }

                data.currentUser.Collections = await dataservice.GetUserCollections(data.currentUser.Id);
                Console.WriteLine("Grabbed user collections.");

                while (data.currentUser.Collections == null || data.currentUser.Collections.Count <= 0)
                {
                    await Task.Delay(2000);
                    Console.WriteLine("Waiting to re-grab collections since they were null.");
                    data.currentUser.Collections = await dataservice.GetUserCollections(data.currentUser.Id);
                }

                SortBy = data.currentUser.CollectionsSort;
                //customOrderBackup = string.Join(",", reorderList.OrderBy(x => x.Order).Select(x => x.Collection.Id)) + ",";
                //Sort();

                Console.WriteLine("CustomOrderBackup: " + customOrderBackup);
                Console.WriteLine("CollectionsSort: " + data.currentUser.CollectionsSort);

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
                    data.currentUser.Collections.RemoveAll(c => c.Id == collection.Id);
                    await dataservice.DeleteCollectionAsync(collection.Id);
                    data.currentUser.CollectionsOrder = data.currentUser.CollectionsOrder.Replace($"{collection.Id},", string.Empty);
                    data.currentUser.Collections = await dataservice.GetUserCollections(data.currentUser.Id);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting collection: {ex.Message}";
            }
        }
    }
}
