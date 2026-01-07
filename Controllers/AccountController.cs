using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.ViewModels.UserViewModels;
using Pronia.Views.Account;

namespace Pronia.Controllers;

public class AccountController(UserManager<AppUser> _userManager,SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration) : Controller
{
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> RegisterAsync( RegisterVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var existUser = await _userManager.FindByNameAsync(vm.Username); 
        if (existUser != null)
        {
            ModelState.AddModelError("Username", "Username already taken");
            return View(vm);
        }
         existUser = await _userManager.FindByEmailAsync(vm.Email);
        if (existUser != null)
        {
            ModelState.AddModelError("EmailAddress", "Username already taken");
            return View(vm);
        }


        AppUser user = new AppUser()
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            UserName = vm.Username,
            Email = vm.Email
        };
       var result= await _userManager.CreateAsync(user,vm.Password);
        if (result.Succeeded==false)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(vm);
        }
        return View();

    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        if (ModelState.IsValid) 
        return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user == null)
        {
            ModelState.AddModelError("Email", "Invalid email or password");
            return View(vm);
        }
        var result = await _userManager.CheckPasswordAsync(user, vm.Password);
        if (result == false)
        {
            ModelState.AddModelError("Password", "Invalid email or password");
            return View(vm);
        }
        await _signInManager.SignInAsync(user, vm.RememberMe);
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
    public async Task<IActionResult> CreateRoles()
    {
        await _roleManager.CreateAsync(new IdentityRole("Admin"));
        await _roleManager.CreateAsync(new IdentityRole("Member"));
        await _roleManager.CreateAsync(new IdentityRole("Moderator"));

        return Ok("Roles Created");
    }

    public async Task<IActionResult> CreateAdminAndModerator()
    {
        var AdminUserVM = _configuration.GetSection("AdminUser").Get<UserVM>();
        var ModeratorUserVM = _configuration.GetSection("ModeratorUser").Get<UserVM>();

        if (AdminUserVM is not null)
        {
            AppUser adminUser = new()
            {
                FirstName = AdminUserVM.FirstName,
                LastName = AdminUserVM.LastName,
                UserName = AdminUserVM.UserName,
                Email = AdminUserVM.Email
            };
            await _userManager.CreateAsync(adminUser, AdminUserVM.Password);
            await _userManager.AddToRoleAsync(adminUser, "Admin");


        }
        if (ModeratorUserVM is not null)
        {

            AppUser moderatorUser = new()
            {
                FirstName = ModeratorUserVM.FirstName,
                LastName = ModeratorUserVM.LastName,
                UserName = ModeratorUserVM.UserName,
                Email = ModeratorUserVM.Email
            };

            await _userManager.CreateAsync(moderatorUser, ModeratorUserVM.Password);
            await _userManager.AddToRoleAsync(moderatorUser, "Moderator");
        }


        return RedirectToAction("Index", "Home");
    }
}
