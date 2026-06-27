using EduTrack.Persistence;
using EduTrack.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();
builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceServices(builder.Configuration);

var app = builder.Build();


await app.MigrateAndSeedDatabaseAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


await app.RunAsync();
