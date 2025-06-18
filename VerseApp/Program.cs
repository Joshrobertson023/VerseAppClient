using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VerseApp;
using MudBlazor.Services;
using DBAccessLibrary;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using DBAccessLibrary.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<DataService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7070/");
});

builder.Services.AddSingleton<Data>();
builder.Services.AddSingleton<CurrentPageStructure>();
builder.Services.AddBlazoredLocalStorageAsSingleton();

builder.Services.AddMudServices(service =>
{
    MudTheme AppTheme = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#1565C0",
             Background = "#EBEBEB"
            //Secondary = "#C3C3C3", // Light gray
            // Info = "rgba(1, 37, 37, 37)" // Dark gray for cards
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#1565C0",
             Background = "#141414",
            Secondary = "#C3C3C3", // Light gray
             Info = "#252525" // Dark gray for cards
             //Text color: EAE9FC
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "0.7rem",
                FontWeight = "300",
                LineHeight = "1.5",
                LetterSpacing = ".02em"
            },
            H1 = new H1Typography
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3rem",
                FontWeight = "500",
                LineHeight = "1.6",
                LetterSpacing = ".0075em"
            },
            H2 = new H2Typography
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = "500",
                LineHeight = "1.5",
                LetterSpacing = ".01em"
            },
            H3 = new H3Typography
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "0.8rem",
                FontWeight = "500",
                LineHeight = "1.7",
                LetterSpacing = ".02em"
            },
            H4 = new H4Typography
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.3rem",
                FontWeight = "500",
                LineHeight = "1.7",
                LetterSpacing = ".02em"
            },
            H5 = new H5Typography
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "16px",
                FontWeight = "200",
                LineHeight = "1.4",
                LetterSpacing = ".0075em"
            },
            H6 = new H6Typography
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = "400",
                LineHeight = "0.8",
                LetterSpacing = ".01em"
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontFamily = new[] { "Inter", "Helvetica", "Arial", "sans-serif" },
                FontSize = "0.85rem",
                FontWeight = "200",
                LineHeight = "1.5",
                LetterSpacing = ".0075em"
            }
        }
    };
builder.Services.AddSingleton(AppTheme);
});

var host = builder.Build();

var dataService = host.Services.GetRequiredService<DataService>();
_ = dataService.Warmup();

await host.RunAsync();
