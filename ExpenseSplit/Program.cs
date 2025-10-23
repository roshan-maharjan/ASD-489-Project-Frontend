using Blazored.LocalStorage;
using ExpenseSplit;
using ExpenseSplit.Handlers;
using ExpenseSplit.Services;
using ExpenseSplit.Services.AuthService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl) || !Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var baseUri))
{
    // Fallback to the current host's address or throw an error
    baseUri = new Uri(builder.HostEnvironment.BaseAddress);
}

builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddHttpClient("WebAPI", client =>
{
    client.BaseAddress = baseUri;
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebAPI"));

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseUri });

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register Blazored Local Storage
builder.Services.AddBlazoredLocalStorage();

// Register the custom Auth State Provider
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
