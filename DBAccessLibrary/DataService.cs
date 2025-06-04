using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DBAccessLibrary.Models;
using DBAccessLibrary;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;

namespace DBAccessLibrary
{
    public partial class DataService
    {
        public bool hasLoaded { get; set; }
        string baseAPIUrl = "";

        private IHttpClientFactory factory;
        public Data data;
        public DataService(IHttpClientFactory _factory)
        {
            factory = _factory;
        }

        private HttpClient http => factory.CreateClient("api");

        public async Task LoadCurrentUserAsync()
        {
            if (hasLoaded) return;

            try
            {
                // Call your "get me" endpoint. It could be "/api/users/me" or "/api/auth/userinfo" 
                // (whatever you set up on the server).
                // If the request is 401, this will throw an HttpRequestException; 
                // we'll catch it and leave CurrentUser = null.
                data.currentUser = await http.GetFromJsonAsync<UserModel>("api/users/currentUser");
            }
            catch
            {
                data.currentUser = null;
            }

            hasLoaded = true;
        }

        public async Task GetAllUsernames()
        {
            data.usernames = await http.GetFromJsonAsync<List<string>>($"{baseAPIUrl}api/user/usernames");
        }

        public async Task LoginUserWithTokenAsync(string token)
        {
            throw new Exception("This is for testing");
            http.DefaultRequestHeaders.Remove("Auth-Token");
            http.DefaultRequestHeaders.Add("Auth-Token", token);
            data.currentUser = await http.GetFromJsonAsync<UserModel>("api/users/currentUser");
        }
    }
}
