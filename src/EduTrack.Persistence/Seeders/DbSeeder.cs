using EduTrack.Domain.Constants;
using EduTrack.Domain.Entities;
using EduTrack.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduTrack.Persistence.Seeders;

public static class DbSeeder
{

    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedRolesAsync(context);
    }

    public static async Task SeedRolesAsync(AppDbContext context)
    {
        var roleIds = RoleConstants.BaseRoles.Select(r => r.Id).ToList();

        if (await context.Roles.AnyAsync(r => roleIds.Contains(r.Id)))
            return;

        await context.Roles.AddRangeAsync(RoleConstants.BaseRoles);
        await context.SaveChangesAsync();
    }
  
}
