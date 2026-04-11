using FacturaPro.Application.Interfaces;
using FacturaPro.Application.Services;
using FacturaPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClientService,    ClientService>();
builder.Services.AddScoped<IProduitService,   ProduitService>();
builder.Services.AddScoped<IFactureService,   FactureService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

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

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<FacturaPro.Web.App>()
    .AddInteractiveServerRenderMode();

app.Run();
