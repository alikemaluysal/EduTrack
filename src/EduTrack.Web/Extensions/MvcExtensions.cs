using EduTrack.Persistence;
using EduTrack.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Web.Extensions;

public static class MvcExtensions
{

    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        await DbSeeder.SeedAsync(context);

    }
}
