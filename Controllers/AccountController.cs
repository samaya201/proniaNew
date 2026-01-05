using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.ViewModels.UserViewModels;

namespace Pronia.Controllers;

public class AccountController(UserManager<AppUser> _userManager) : Controller
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
}
