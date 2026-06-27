using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduTrack.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }

}
