using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace VerseApp
{
    /// <summary>
    /// A DelegatingHandler that reads the saved JWT from localStorage
    /// and, if present, attaches it to outgoing HttpClient requests as
    /// an "Authorization: Bearer {token}" header.
    /// </summary>
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigation;

        public JwtAuthorizationMessageHandler(ILocalStorageService localStorage, NavigationManager navigation)
        {
            _localStorage = localStorage;
            _navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1) Look for the JWT in localStorage under "authToken" (or whatever key you choose)
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // 2) If 401 Unauthorized (token expired or invalid), remove it and redirect to login
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _localStorage.RemoveItemAsync("authToken");
                _navigation.NavigateTo("authentication/login");
            }

            return response;
        }
    }
}
