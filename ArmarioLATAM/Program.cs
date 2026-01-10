using ArmarioLATAM.Components;
using ArmarioLATAM.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 👉 HttpClient para llamar a la API
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5215/"); // puerto de la API
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IKitService, KitService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5215/"); // tu API
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Registrar ProtectedSessionStorage
builder.Services.AddScoped<ProtectedSessionStorage>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
