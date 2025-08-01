using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Aos_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Aos_1.Controller;

public class UserRegistrationModel
{ 
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    
    // public string Role { get; set; }
}

public class LoginModel
{ 
    public string Email { get; set; }
    public string Password { get; set; }
    
}
public static class IdentityUserEndpoints
{
    public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapPost("/signup", CreateUser);
        app.MapPost("/signin", SignIn);
        return app;
    }

    [AllowAnonymous]
    private static async Task<IResult> CreateUser(UserManager<AppUser> userManager,
        [FromBody] UserRegistrationModel userRegistrationModel)
    {
        
        var fullNameExists = await userManager.Users.FirstOrDefaultAsync(x=> x.FullName == userRegistrationModel.FullName);

        if (fullNameExists != null)
        {
            return Results.BadRequest(new { message = "Username already exists" });
        }

        AppUser user = new AppUser()
        {
            UserName = userRegistrationModel.Email,
            Email = userRegistrationModel.Email,
            FullName = userRegistrationModel.FullName,
        };
        var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
        await userManager.AddToRoleAsync(user, "SuperAdmin");
        if (result.Succeeded)
            return Results.Ok(result);
        else
        {
            var errors = result.Errors.Select(e => e.Description).ToList();

            string message = "Something went wrong";

            // Email stored in UserName column
            if (errors.Any(e => e.ToLower().Contains("username") && e.ToLower().Contains("is already taken")))
            {
                message = "Email already exists";
            }

            else if (errors.Any(e => e.ToLower().Contains("fullname") && e.ToLower().Contains("is already taken")))
            {
                message = "Username already exists";
            }

            return Results.BadRequest(new { message });
        }
    }

    [AllowAnonymous]
    private static async Task<IResult> SignIn (UserManager<AppUser> userManager, 
            [FromBody] LoginModel LoginModel, IOptions<AppSettings> appSettig)
        {
            var user = await userManager.FindByEmailAsync(LoginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, LoginModel.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettig.Value.JWTSecret));

                var tokenDescriptor = new SecurityTokenDescriptor
                {

                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, roles.First()),
                    }),
                    Expires = DateTime.UtcNow.AddDays(10),
                    SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Results.Ok(new { token });
            }
            else
            {
                return Results.BadRequest("Invalid username or password");
            }
        }
}
