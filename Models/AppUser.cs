using Microsoft.AspNetCore.Identity;

namespace Pronia.Models;

public class AppUser:IdentityUser
{
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    /*public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string EmailHash { get; set; }
    public string PhoneNumber { get; set; }*/
}
