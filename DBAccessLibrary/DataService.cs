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
        string baseAPIUrl = "https://localhost:7070/api";

        private IHttpClientFactory factory;
        public Data data;
        public DataService(IHttpClientFactory _factory, Data _data)
        {
            factory = _factory;
            data = _data;
        }

        private HttpClient http => factory.CreateClient("api");

        public async Task<UserModel> GetUser(string username)
        {
            try
            {
                var response = await http.PostAsJsonAsync($"api/user/getuser", username);
                return await response.Content.ReadFromJsonAsync<UserModel>();
            }
            catch
            {
                return null;
            }
        }

        public async Task GetAllUsernames()
        {
            data.usernames = new List<string>();
            data.usernames = await http.GetFromJsonAsync<List<string>>($"api/user/usernames");
        }

        public async Task<string> GetPasswordHash(string username)
        {
            var response = await http.PostAsJsonAsync($"api/user/passwordhash", username);
            return await response.Content.ReadFromJsonAsync<string>();
        }

        public async Task AddUserAsync(UserModel newUser)
        {
            var response = await http.PostAsJsonAsync($"api/user/adduser", newUser);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API AddUser failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task<UserModel> LoginUserWithTokenAsync(string token)
        {
            var response = await http.PostAsJsonAsync($"api/user/loginwithtoken", token);

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"LoginWithToken FAILED: {response.StatusCode} ⇒ {raw}");
                return null;
            }
            try
            {
                var user = await response.Content.ReadFromJsonAsync<UserModel>();
                return user;
            }
            catch (Exception ex)
            {
                // If parsing fails, dump the body again for debugging
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to parse JSON: {ex.Message}\nBody was:\n{raw}");
                return null;
            }
        }
    }
}
