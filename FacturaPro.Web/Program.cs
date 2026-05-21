using FacturaPro.Application.Interfaces;
using FacturaPro.Application.Services;
using FacturaPro.Infrastructure.Data;
using FacturaPro.Web.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.Redirect("/login");
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IClientService,    ClientService>();
builder.Services.AddScoped<IProduitService,   ProduitService>();
builder.Services.AddScoped<IFactureService,   FactureService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthService,      AuthService>();

builder.Services.AddScoped<SessionUtilisateur>();
builder.Services.AddScoped<AuthenticationStateProvider, FacturaAuthStateProvider>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<FacturaPro.Web.App>()
    .AddInteractiveServerRenderMode();

app.Run();
