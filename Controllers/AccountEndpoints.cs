using System.Security.Claims;
using Aos_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Aos_1.Controller;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/GetProfile", GetProfile);
        return app;
    }
    
    [Authorize]
    private static async Task<IResult> GetProfile(ClaimsPrincipal user, UserManager<AppUser> userManager)
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