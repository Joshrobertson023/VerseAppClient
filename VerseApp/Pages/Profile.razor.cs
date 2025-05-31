using DBAccessLibrary;
using DBAccessLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace VerseApp.Pages
{
    public partial class Profile : ComponentBase
    {
        private bool loading { get; set; } = false;
        [Inject]
        DataService data { get; set; }

        private void Login()
        {

        }

        private void CreateAccount()
        {

        }
    }
}
