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
using VerseAppAPI.Models;

namespace DBAccessLibrary
{
    public partial class DataService
    {
        string baseAPIUrl = "https://localhost:7070/api";

        private HttpClient http;
        public Data data;
        public DataService(HttpClient _http, Data _data)
        {
            http = _http;
            data = _data;
        }


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

        #region User Management
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

        public async Task<int> CheckIfUsernameExists(string username)
        {
            var response = await http.PostAsJsonAsync($"api/user/checkifusernameexists", username);
            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CheckIfUsernameExists failed ({(int)response.StatusCode}): {raw}");
                return -1;
            }
            try
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            catch (Exception ex)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to parse JSON in CheckIfUsernameExists: {ex.Message}\nBody was:\n{raw}");
                return -1;
            }
        }

        public async Task<List<string>> GetAllUsernamesAsync()
        {
            var response = await http.GetAsync("api/user/getallusernames");
            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetAllUsernames failed ({(int)response.StatusCode}): {raw}");
                return null;
            }
            try
            {
                return await response.Content.ReadFromJsonAsync<List<string>>();
            }
            catch (Exception ex)
            {
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to parse JSON in GetAllUsernames: {ex.Message}\nBody was:\n{raw}");
                return null;
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
            var response = await http.PostAsJsonAsync($"api/user/verifytoken", new {Username = username, Token = token});
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
            var response = await http.PostAsJsonAsync($"api/user/updatepassword", new {Id = userId, PasswordHash = passwordHash});
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API UpdateUserPassword failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task GetRecoveryInfo()
        {
            var list = await http.GetFromJsonAsync<List<RecoveryInfo>>("api/user/getrecoveryinfo");

            data.recoveryInfo = list ?? new List<RecoveryInfo>();
        }

        public async Task SendResetLink(string username, string email)
        {
            var dto = new {Username = username, Email = email };

            var response = await http.PostAsJsonAsync("api/user/sendresetlink", dto);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Verse Management
        public async Task<UserVerse> GetUserVerseByReferenceAsync(Reference reference)
        {
            var response = await http.PostAsJsonAsync($"api/verse/getuserversebyreference", reference);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetUserVerseByReference failed ({(int)response.StatusCode}): {payload}");
            }
            return await response.Content.ReadFromJsonAsync<UserVerse>();
        }

        public async Task<List<Verse>> GetUserVerseByKeywordsAsync(List<string> keywords)
        {
            var response = await http.PostAsJsonAsync($"api/verse/getuserversebykeywords", keywords);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetUserVerseByKeywords failed ({(int)response.StatusCode}): {payload}");
            }
            return await response.Content.ReadFromJsonAsync<List<Verse>>();
        }

        public async Task AddNewCollection(Collection collection)
        {
            var response = await http.PostAsJsonAsync($"api/verse/addnewcollection", collection);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API AddNewCollection failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task<List<Collection>> GetUserCollections(int userId)
        {
            var response = await http.PostAsJsonAsync($"api/verse/getusercollections", userId);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetUserCollections failed ({(int)response.StatusCode}): {payload}");
            }
            return await response.Content.ReadFromJsonAsync<List<Collection>>();
        }

        public async Task<Collection> GetVersesByCollectionAsync(Collection collection)
        {
            var response = await http.PostAsJsonAsync($"api/verse/getversesbycollection", collection);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetVersesByCollectionId failed ({(int)response.StatusCode}): {payload}");
            }
            return await response.Content.ReadFromJsonAsync<Collection>();
        }

        public async Task DeleteCollectionAsync(int collectionId)
        {
            var response = await http.PostAsJsonAsync($"api/verse/deletecollection", collectionId);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API DeleteCollection failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task<List<Notification>> GetUserNotifications(string username)
        {
            var response = await http.PostAsJsonAsync($"api/user/getusernotifications", username);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetUserNotifications failed ({(int)response.StatusCode}): {payload}");
            }

            List<Notification> unseenNotifications = new List<Notification>();
            List<Notification> notifications = new List<Notification>();

            notifications = await response.Content.ReadFromJsonAsync<List<Notification>>();

            foreach (var notification in notifications)
            {
                if (notification.Seen == 0)
                    unseenNotifications.Add(notification);
            }

            data.currentUser.NotificationsUnseen = unseenNotifications;

            return notifications;
        }

        public async Task MarkNotificationAsRead(int notificationId)
        {
            var response = await http.PostAsJsonAsync($"api/user/marknotificationasread", notificationId);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API MarkNotificationAsRead failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task DeleteNotification(int notificationId)
        {
            var response = await http.PostAsJsonAsync($"api/user/deletenotification", notificationId);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API DeleteNotification failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task SendNotification(Notification notification)
        {
            var response = await http.PostAsJsonAsync($"api/user/sendnotification", notification);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API SendNotification failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task SendEmailNotification(Notification notification)
        {
            var response = await http.PostAsJsonAsync($"api/user/sendemailnotification", notification);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API SendEmailNotification failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task<Collection> GetCollectionAsync(int collectionId)
        {
            var response = await http.PostAsJsonAsync($"api/verse/getcollection", collectionId);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API GetCollection failed ({(int)response.StatusCode}): {payload}");
            }
            return await response.Content.ReadFromJsonAsync<Collection>();
        }

        public async Task AddUserVersesToNewCollection(List<UserVerse> userVerses)
        {
            var response = await http.PostAsJsonAsync($"api/verse/adduserversestonewcollection", userVerses);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API AddUserVerses failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task TogglePinCollection(Collection collection)
        {
            var response = await http.PostAsJsonAsync($"api/verse/togglepincolllection", collection);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API TogglePinCollection failed ({(int)response.StatusCode}): {payload}");
            }
        }

        public async Task UpdateCollectionsOrder(OrderInfo order)
        {
            var response = await http.PostAsJsonAsync($"api/verse/updatecollectionsorder", order);
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                throw new Exception($"API UpdateCollectionsOrder failed ({(int)response.StatusCode}): {payload}");
            }
        }
        #endregion
    }
}
