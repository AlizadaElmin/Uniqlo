using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uniqlo.Extensions;
using Uniqlo.Models;
using Uniqlo.ViewModels.Auths;
using Uniqlo.Views.Account.Enums;

namespace Uniqlo.Controllers;

public class AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager,RoleManager<IdentityRole> _roleManager):Controller
{
    private bool isAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;
    public IActionResult Register()
    {
        if (isAuthenticated) return RedirectToAction("Index", "Home");
        return View(); 
    }
    [HttpPost]

    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (isAuthenticated) return RedirectToAction("Index", "Home");
        if (!ModelState.IsValid)
            return View();

        User user = new User()
        {
            Fullname = vm.Fullname,
            Email = vm.Email,
            UserName = vm.Username
        };
        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
            {
               ModelState.AddModelError("",err.Description);
            }
            return View();
        }
        var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));

        if (!roleResult.Succeeded)
        {
            foreach (var err in roleResult.Errors)
            {
                ModelState.AddModelError("",err.Description);
            }
            return View();
        }

        return View();
    }

    // public async Task<IActionResult> Method()
    // {
    //     foreach (Roles item in Enum.GetValues(typeof(Roles)))
    //     {
    //        await _roleManager.CreateAsync(new IdentityRole(item.GetRole()));
    //     }
    //
    //     return Ok();
    // }
    
    public IActionResult Login()
    {
        if (isAuthenticated) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm,string? returnUrl = null)
    {
        if (isAuthenticated) return RedirectToAction("Index", "Home");
        if (!ModelState.IsValid) return View();

        User? user = null;
        if (vm.UsernameOrEmail.Contains("@"))
            user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
        else
            user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);

        if (user == null)
        { 
            ModelState.AddModelError("", "Username or password is wrong!");
            return View();
        }
        
        var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("","Wait untill "+ user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("","Username or email is wrong!");
            }
            return View();
        }

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index",new{area="Admin",controller="Dashboard"});
            }
            return RedirectToAction("Index","Home");
        }
        return LocalRedirect(returnUrl);
    }
    
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}