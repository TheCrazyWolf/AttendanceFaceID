using MudBlazor.Services;
using AttendanceFaceID.Components;
using AttendanceFaceID.Services.Services;
using AttendanceFaceID.Storage;
using AttendanceFaceID.Storage.Context;
using Blazored.LocalStorage;
using ClientSamgk;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddScoped<AttendanceContext>();
builder.Services.AddScoped<AttendanceMainRepo>();
builder.Services.AddScoped<StudentsService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddSingleton<ClientSamgkApi>();
builder.Services.AddBlazoredLocalStorage();
/*builder.WebHost.ConfigureKestrel((httpClient, options) =>
{
    options.Listen(IPAddress.Any, httpClient.Configuration.GetValue<int?>("Port") ?? 5005);
});*/

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AttendanceContext>();
    dbContext.Database.Migrate();
}

app.Run();