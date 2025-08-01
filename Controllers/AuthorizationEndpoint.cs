using System.Security.Claims;
using Aos_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Aos_1.Controller;

public static class AuthorizationEndpoint
{
    public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/GetSuperAdmin", SuperAdmin);
        app.MapGet("/GetProductAdmin", ProductAdmin);
        app.MapGet("/GetUserAdmin", UserAdmin);
        return app;
    }

    
    
    [Authorize(Roles = "SuperAdmin")]
    
    private static async Task<IResult> SuperAdmin(ClaimsPrincipal user, UserManager<AppUser> userManager)
    {
        var userID = user.Claims.First(x => x.Type =="UserId").Value;
        var userDetails = await userManager.FindByIdAsync(userID);
        var roles = await userManager.GetRolesAsync(userDetails);
        return Results.Ok( new
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Roles = roles,
        });
    }
    
    [Authorize(Roles = "ProductAdmin,SuperAdmin")]
    private static async Task<IResult> ProductAdmin(ClaimsPrincipal user, UserManager<AppUser> userManager)
    {
        var userID = user.Claims.First(x => x.Type =="UserId").Value;
        var userDetails = await userManager.FindByIdAsync(userID);
        var roles = await userManager.GetRolesAsync(userDetails);
        return Results.Ok( new
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Roles = roles,
        });
    }

    
    [Authorize(Roles = "UserAdmin,SuperAdmin")]
    private static async Task<IResult> UserAdmin(ClaimsPrincipal user, UserManager<AppUser> userManager)
    {
        var userID = user.Claims.First(x => x.Type =="UserId").Value;
        var userDetails = await userManager.FindByIdAsync(userID);
        var roles = await userManager.GetRolesAsync(userDetails);
        return Results.Ok( new
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Roles = roles,
        });
    }

}