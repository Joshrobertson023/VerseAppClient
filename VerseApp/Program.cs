using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using VerseApp;
using MudBlazor.Services;
using DBAccessLibrary;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7070/");
    // <-- Ensure this matches the URL your API is running on
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddMudServices();
builder.Services.AddScoped<DataService>();
builder.Services.AddSingleton<Data>();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();
