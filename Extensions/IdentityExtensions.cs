using System.Text;
using Aos_1.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Aos_1.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityHandlerandStore(this IServiceCollection services)
    {
        services.AddIdentityApiEndpoints<AppUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
        return services;
    }

    public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
    { 
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = true;
        });
        return services;
    }
    
    //Auth = Authentication + Authorization
    public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
    { 
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            　　　.AddJwtBearer(y => 
               {
            　　　y.SaveToken = false; 
              　　 y.TokenValidationParameters = new TokenValidationParameters
           　 　　{
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["AppSettings:JWTSecret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });
        return services;
    }
    
    public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
    
}