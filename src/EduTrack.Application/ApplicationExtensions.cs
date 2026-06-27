using BlogApp.Application.Services.Concrete;
using EduTrack.Application.BusinessRules;
using EduTrack.Application.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduTrack.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IAuthService, AuthService>();


        //TODO: business rule tipine sahip tüm classlar otomatik inject edilsin
        services.AddScoped<AuthBusinessRules>();

        return services;
    }

}
