using BlogApp.Application.Services.Concrete;
using Core.BusinessRules;
using EduTrack.Application.Services.Abstract;
using EduTrack.Application.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace EduTrack.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStreamPostService, StreamPostService>();
        services.AddScoped<ICourseMaterialService, CourseMaterialService>();


        services.Scan(s =>
             s.FromAssemblies(Assembly.GetExecutingAssembly())
             .AddClasses(classes => classes.AssignableTo<IBusinessRule>())
             .AsSelf()
             .WithScopedLifetime()
             );

        return services;
    }

}
