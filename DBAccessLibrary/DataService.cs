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

        public event Action<string> OnStatusChanged;
        protected void RaiseStatus(string message) => OnStatusChanged?.Invoke(message);

        public async Task Warmup()
        {
            try
            {
                var response = await http.GetAsync("api/user/warmup");

                if (!response.IsSuccessStatusCode)
                {
                    var rawError = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Warmup failed ({(int)response.StatusCode}): {rawError}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Warmup: {ex.Message}");
            }
        }

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
            try
            {
                var response = await http.GetAsync("api/user/usernames");

                if (!response.IsSuccessStatusCode)
                {
                    var rawError = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GetAllUsernames failed ({(int)response.StatusCode}): {rawError}");

                    data.usernames = new List<string>();
                    return;
                }

                try
                {
                    data.usernames = await response.Content.ReadFromJsonAsync<List<string>>();
                }
                catch (Exception jsonEx)
                {
                    var rawBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to parse JSON in GetAllUsernames: {jsonEx.Message}\nBody was:\n{rawBody}");

                    data.usernames = new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAllUsernames: {ex.Message}");
                data.usernames = new List<string>();
            }
        }


        public async Task<string> GetPasswordHash(string username)
        {
            var response = await http.PostAsJsonAsync("api/user/passwordhash", username);

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetPasswordHash failed ({(int)response.StatusCode}): {raw}");
                return null;
            }
            try
            {
                return await response.Content.ReadFromJsonAsync<string>();
            }
            catch (Exception ex)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to parse JSON in GetPasswordHash: {ex.Message}\nBody was:\n{raw}");
                return null;
            }
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
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to parse JSON: {ex.Message}\nBody was:\n{raw}");
                return null;
            }
        }

        public async Task<string> GetSecurityQuestion(string username)
        {
            var response = await http.PostAsJsonAsync($"api/user/securityquestion", username);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetSecurityQuestion failed ({(int)response.StatusCode}): {payload}");
            }
            return await response.Content.ReadFromJsonAsync<string>();
        }

        public async Task<bool> VerifyTokenDBAsync(string username, string token)
        {
            RecoveryInfo recovery = new RecoveryInfo();
            recovery.Username = username;
            recovery.Token = token;
            var response = await http.PostAsJsonAsync($"api/user/verifytoken", recovery);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API VerifyToken failed ({(int)response.StatusCode}): {payload}");
            }
            bool verified = await response.Content.ReadFromJsonAsync<bool>();
            return verified;
        }

        public async Task PutResetToken(string username, string token)
        {
            RecoveryInfo recovery = new RecoveryInfo();
            recovery.Username = username;
            recovery.Token = token;
            var response = await http.PostAsJsonAsync($"api/user/putresettoken", recovery);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API PutResetToken failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task<int> GetIdFromUsername(string username)
        {
            var response = await http.PostAsJsonAsync($"api/user/getidfromusername", username);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetIdFromUsername failed ({(int)response.StatusCode}): {payload}");
            }
            int userId = await response.Content.ReadFromJsonAsync<int>();
            return userId;
        }

        public async Task UpdateUserPassword(int userId, string passwordHash)
        {
            RecoveryInfo recovery = new RecoveryInfo();
            recovery.Id = userId;
            recovery.PasswordHash = passwordHash;
            var response = await http.PostAsJsonAsync($"api/user/updatepassword", recovery);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API UpdateUserPassword failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task GetRecoveryInfo()
        {
            data.recoveryInfo = await http.GetFromJsonAsync<List<RecoveryInfo>>($"api/user/getrecoveryinfo");
        }
    }
}
