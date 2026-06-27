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
        var baseRoles = RoleConstants.BaseRoles;

        foreach (var role in baseRoles)
        {
            if (!await context.Roles.AnyAsync(r => r.Id == role.Id))
            {
                await context.Roles.AddAsync(role);
            }
        }

        await context.SaveChangesAsync();
    }
  
}
