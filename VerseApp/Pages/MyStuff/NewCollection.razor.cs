using DBAccessLibrary;
using DBAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using VerseAppAPI.Models;

namespace VerseApp.Pages.MyStuff
{
    public partial class NewCollection : ComponentBase
    {
        [Inject]
        NavigationManager nav { get; set; }
        [Inject]
        DataService dataservice { get; set; }
        [Inject]
        Data data { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        private int progress;
        private string overlayMessage { get; set; }
        private string errorMessage { get; set; }
        private string message { get; set; }

        private bool overlayVisible = false;

        private void ToggleOverlay()
        {
            overlayVisible = !overlayVisible;
            StateHasChanged();
        }

        private void CloseDrawer()
        {

        }

        private Anchor anchor;
        private bool open;
        private bool overlayAutoClose = false;
        private void OpenDrawer(Anchor _anchor)
        {
            open = !open;
            anchor = _anchor;
        }


        private async Task Back_Click()
        {
            await JSRuntime.InvokeVoidAsync("history.back");
        }

        private string collectionName { get; set; }
        private List<UserVerse> collectionVerses { get; set; } = new();
        private UserVerse drawerVerse { get; set; }
        private string readableReference { get; set; }
        private IEnumerable<int> selectedVerses;
        private bool overflow = false;
        private string search { get; set; }
        private string userSearch { get; set; }
        private string _book;
        private int _chapter;
        private int _verse;
        private int numChapters;
        private int numVerses;
        private string drawerMessage;

        private string book
        {
            get => _book;
            set
            {
                if (_book != value)
                {
                    _book = value;
                    OnBookChange();
                }
            }
        }

        private int chapter
        {
            get => _chapter;
            set
            {
                if (_chapter != value)
                {
                    _chapter = value;
                    OnChapterChange();
                }
            }
        }
        private int verse
        {
            get => _verse;
            set => _verse = value;
        }

        private IEnumerable<int> verses;
        private IEnumerable<int> Verses { get { return verses; } set { verses = value; } }

        private void OnBookChange()
        {
            chapter = 0;
            Verses = Array.Empty<int>();
            selectedVerses = Array.Empty<int>();

            numChapters = BibleStructure.GetNumberChapters(book);
            numVerses = 0;
            errorMessage = "";
        }

        private void OnChapterChange()
        {
            Verses = Array.Empty<int>();
            selectedVerses = Array.Empty<int>();

            numVerses = BibleStructure.GetNumberVerses(book, chapter);
            errorMessage = "";
        }

        private async Task Apply_Click()
        {
            message = "";
            if (!selectedVerses.Any())
            {
                errorMessage = "Please select at least one verse.";
                return;
            }

            try
            {
                overflow = false;
                search = string.Empty;
                drawerVerse = await dataservice.GetUserVerseByReferenceAsync(new Reference { Book = book, Chapter = chapter, Verses = selectedVerses.ToList() });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }

        private async Task Search_Click()
        {
            if (string.IsNullOrEmpty(userSearch))
            {
                errorMessage = "Please enter a reference or keyword(s) to search.";
                return;
            }

            //try
            //{
            string search = userSearch.Trim();
            search += " "; // Terminate with special character
                           // Check for proper format: book chapter verses
                           // if first word is not a book name, then search by keyword(s)
            string firstWord = string.Empty;
            int i = 0;
            // Get the book
            for (i = 0; i < search.Length; i++)
            {
                char letter = search[i];
                if (i > 1)
                    if (char.IsWhiteSpace(letter) || char.IsPunctuation(letter))
                        break;
                firstWord += letter;
                if (i + 1 == search.Length)
                    throw new Exception("Invalid search format.");
            }

            firstWord = firstWord.Trim();
            firstWord = firstWord.ToLower();

            // Check if the first word is a book of the Bible
            bool found = false;
            foreach (var book in data.booksOfBibleSearch)
            {
                if (!found)
                {
                    foreach (var searchTerm in book.Value)
                    {
                        if (!found)
                        {
                            if (firstWord == searchTerm)
                            {
                                firstWord = book.Key;
                                found = true;
                            }
                        }
                    }
                }
            }

            if (found)
            {
                // Search by reference
                drawerMessage = "Searching by reference: " + search;
                string _book = firstWord;
                int _chapter = 0;
                string chapterString = string.Empty;

                // You know the next number will be the chapter
                // You know after that, it will be all verses

                // Get the chapter
                for (; i < search.Length; i++)
                {
                    if (char.IsNumber(search[i]))
                    {
                        chapterString += search[i];
                        if (i < search.Length - 1)
                        {
                            if (!char.IsNumber(search[i + 1]))
                                break;
                            if (i + 1 == search.Length)
                                throw new Exception("Invalid search format.");
                        }
                    }
                }
                _chapter = int.TryParse(chapterString, out int chapterResult) ? chapterResult : throw new Exception("Error getting chapter.");
                Console.WriteLine($"Book: {_book}, Chapter: {_chapter}");

                List<int> _verses = new List<int>();
                string numberString = string.Empty;
                List<string> verseRanges = new();
                string verseRange = string.Empty;

                // Get the verses
                i++;
                verseRange = ",";
                for (; i < search.Length; i++)
                {
                    // Get next verse number as string
                    numberString = "";
                    for (; i < search.Length; i++)
                    {
                        // Add to numberString until hit non number
                        // If verseRange ends in -, add the numberString to verseRange. Add it to list, reset verseRange
                        // Else:
                        // Check if - or ,
                        // If - then add numberString to verseRange, reset numberString
                        // If , or a space then add the numberString to _verses and reset numberString
                        if (char.IsNumber(search[i]))
                        {
                            numberString += search[i];
                            if (i < search.Length - 1)
                                if (!char.IsNumber(search[i + 1]))
                                    break;
                        }
                    }

                    if (i < search.Length - 1)
                        Console.WriteLine("Search[i]: " + search[i]);
                    if (verseRange[verseRange.Length - 1] == '-')
                    {
                        verseRange += numberString;
                        verseRanges.Add(verseRange);
                        Console.WriteLine("verseRange ends with '-'. Adding" + numberString + " to verseRange: " + verseRange);
                        numberString = string.Empty;
                        verseRange = ",";
                    }
                    else
                    {
                        if (i < search.Length - 1)
                        {
                            if (search[i + 1] == '-')
                            {
                                verseRange += numberString + "-";
                                Console.WriteLine("verseRange does not end with '-'. Adding" + numberString + " + \"-\" to verseRange: " + verseRange);
                                numberString = string.Empty;
                            }
                            else// if (search[i+1] == ',' || search[i+1] == ' ')
                            {
                                Console.WriteLine("Adding to verses, numberString: " + numberString);
                                _verses.Add(int.TryParse(numberString, out int verseResult) ? verseResult : throw new Exception("Error getting verse."));
                                numberString = string.Empty;
                            }
                            Console.WriteLine("verseRange: " + verseRange);
                            Console.WriteLine("search[i]: " + search[i]);
                        }
                    }
                }

                // Get all verses from ranges
                foreach (var _range in verseRanges)
                {
                    string range = _range + ",";
                    string firstNumber = string.Empty;
                    string secondNumber = string.Empty;
                    int j = 0;
                    for (; j < range.Length; j++)
                    {
                        if (char.IsNumber(range[j]))
                        {
                            firstNumber += range[j];
                            if (j < range.Length - 1)
                                if (!char.IsNumber(range[j + 1]))
                                    break;
                        }
                    }
                    j++;
                    for (; j < range.Length; j++)
                    {
                        if (char.IsNumber(range[j]))
                        {
                            secondNumber += range[j];
                            if (j < range.Length - 1)
                                if (!char.IsNumber(range[j + 1]))
                                    break;
                        }
                    }

                    int first = int.TryParse(firstNumber, out int firstResult) ? firstResult : throw new Exception("Error getting first verse in range.");
                    int second = int.TryParse(secondNumber, out int secondResult) ? secondResult : throw new Exception("Error getting second verse in range.");

                    Console.WriteLine($"Adding verses from range: {first} to {second}");
                    if (first > second)
                    {
                        int temp = first;
                        first = second;
                        second = temp;
                    }

                    for (int k = first; k <= second; k++)
                    {
                        if (!_verses.Contains(k))
                            _verses.Add(k);
                    }

                }

                Console.WriteLine($"Book: {_book}, Chapter: {_chapter}, Verses: {string.Join(", ", _verses)}");
                drawerMessage = "Searching by reference: " + userSearch;
                try
                {
                    Console.WriteLine("_verses.Count: " + _verses.Count);
                    if (_verses.Count <= 0)
                    {
                        Console.WriteLine("Getting whole chapter.");
                        Console.WriteLine("BibleStructure.GetNumberVerses(data.booksOfBible[i], _chapter): " + BibleStructure.GetNumberVerses(data.booksOfBible[i], _chapter));
                        // Get the whole chapter
                        int totalVerses = BibleStructure.GetNumberVerses(_book, _chapter);
                        for (int j = 1; j <= totalVerses; j++)
                        {
                            _verses.Add(j);
                        }
                    }
                    Console.WriteLine("_verses: " + string.Join(", ", _verses));
                    _verses.Sort();
                    drawerVerse = await dataservice.GetUserVerseByReferenceAsync(new Reference { Book = _book, Chapter = _chapter, Verses = _verses });
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("request timed out"))
                    {
                        await Search_Click();
                        Console.WriteLine("Request timed out.");
                        return;
                    }
                    else
                    {
                        drawerMessage = "Improper reference format." + ex.Message;
                    }
                }
            }
            else
            {
                // Search by keywords / exact phrase
                drawerMessage = "Searching by keywords: " + search;

                List<string> keywords = new List<string>();

                string keyword = string.Empty;
                for (int k = 0; k < search.Length; k++)
                {
                    if (char.IsLetter(search[k]))
                    {
                        keyword += search[k];
                        if (k < search.Length - 1)
                        {
                            if (!char.IsLetter(search[k + 1]))
                            {
                                keywords.Add(keyword);
                                Console.WriteLine("Adding keyword: " + keyword);
                                keyword = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                Console.WriteLine("Keywords: " + string.Join(", ", keywords));
                drawerVerse = new UserVerse();
                drawerVerse.Verses = await dataservice.GetUserVerseByKeywordsAsync(keywords);
            }
            //}
            //catch (Exception ex)
            //{
            //    errorMessage = ex.Message;
            //    return;
            //}
        }

        private async Task CheckForEnterKey(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Search_Click();
            }
        }

        private void AddPassage_Click()
        {

        }

        private void CreateCollection_Click() // User gets max of 50 collections, with 100 verses max each
        {

        }
    }
}
