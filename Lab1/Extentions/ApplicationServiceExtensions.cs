using FluentValidation;
using Lab1.Auth.Model;
using Lab1.Data;
using Microsoft.AspNetCore.Identity;

namespace Lab1.Auth;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddDbContext<ForumDbContext>();
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddTransient<JwtTokenService>();
        services.AddScoped<AuthDbSeeder>();

        services.AddIdentity<ForumRestUser, IdentityRole>()
            .AddEntityFrameworkStores<ForumDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}