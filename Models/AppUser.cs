using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Aos_1.Models;

public class AppUser:IdentityUser   
{
    [PersonalData]
    [Column(TypeName = "nvarchar(150)")]
    public string FullName { get; set; }
    
}