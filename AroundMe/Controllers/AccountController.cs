using AroundMe.Models;
using AroundMe.Repositories;
using AroundMe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AroundMe.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly ILegacyUserRepository _legacyUsers;

    public AccountController(UserManager<ApplicationUser> users,
        SignInManager<ApplicationUser> signIn, ILegacyUserRepository legacyUsers)
    {
        _users = users;
        _signIn = signIn;
        _legacyUsers = legacyUsers;
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _legacyUsers.FindByEmailAsync(model.Email.Trim()) != null)
        {
            ModelState.AddModelError("", "This email already exists in the D1 database.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Username.Trim(),
            Email = model.Email.Trim(),
            FullName = model.FullName.Trim(),
            City = model.City?.Trim()
        };

        var result = await _users.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        try
        {
            user.LegacyUserId = await _legacyUsers.CreateAsync(user.FullName,
                user.UserName!, user.Email!);
            var update = await _users.UpdateAsync(user);
            if (!update.Succeeded)
            {
                throw new InvalidOperationException("Could not link the legacy profile.");
            }
        }
        catch (Exception)
        {
            await _users.DeleteAsync(user);
            ModelState.AddModelError("", "Could not create the linked profile. Check the database.");
            return View(model);
        }

        await _signIn.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _users.FindByEmailAsync(model.Email.Trim());
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var result = await _signIn.PasswordSignInAsync(user, model.Password, false, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
