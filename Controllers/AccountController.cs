using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.ViewModels.UserViewModels;

namespace Pronia.Controllers;

public class AccountController(UserManager<AppUser> _userManager,SignInManager<AppUser> _signInManager) : Controller
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

}
