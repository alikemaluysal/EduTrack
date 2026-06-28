using BlogApp.Application.Services.Concrete;
using EduTrack.Application.BusinessRules;
using EduTrack.Application.Services.Abstract;
using EduTrack.Application.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduTrack.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICourseService, CourseService>();


        //TODO: business rule tipine sahip tüm classlar otomatik inject edilsin
        services.AddScoped<AuthBusinessRules>();
        services.AddScoped<UserBusinessRules>();
        services.AddScoped<CourseBusinessRules>();

        return services;
    }

}
