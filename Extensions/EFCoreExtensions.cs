using Aos_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Aos_1.Extensions;

public static class EFCoreExtensions
{
    public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        return services;
    }
}