using AroundMe.Models;
using AroundMe.Services;
using AroundMe.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AroundMe.Controllers;
public class AccountController(UserManager<ApplicationUser> users,SignInManager<ApplicationUser> signIn,PlaceRepository places):Controller
{
    [HttpGet,AllowAnonymous] public IActionResult Register()=>View(new RegisterViewModel());
    [HttpPost,AllowAnonymous,ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if(!ModelState.IsValid) return View(model);
        if(places.FindLegacyUser(model.Email.Trim())!=null)
        {
            ModelState.AddModelError("", "This email exists in the D1 database. Create a new account with a different email, or migrate the existing account.");
            return View(model);
        }
        var user=new ApplicationUser
        {
            UserName=model.Username.Trim(),Email=model.Email.Trim(),FullName=model.FullName.Trim()
        };
        var result=await users.CreateAsync(user,model.Password);
        if(!result.Succeeded)
        {
            foreach(var error in result.Errors)ModelState.AddModelError("",error.Description);
            return View(model);
        }
        try
        {
            user.LegacyUserId=places.CreateLegacyUser(user.FullName,user.UserName!,user.Email!);
            var update=await users.UpdateAsync(user);
            if(!update.Succeeded)throw new InvalidOperationException(string.Join("; ",update.Errors.Select(e=>e.Description)));
        }
        catch(Exception)
        {
            await users.DeleteAsync(user);
            ModelState.AddModelError("","Could not create the linked D1 profile. Check database and username uniqueness.");
            return View(model);
        }
        await signIn.SignInAsync(user,isPersistent:false);
        return RedirectToAction("Index","Home");
    }
    [HttpGet,AllowAnonymous] public IActionResult Login(string? returnUrl=null)=>View(new LoginViewModel{ReturnUrl=returnUrl});
    [HttpPost,AllowAnonymous,ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginViewModel model)
    {
        if(!ModelState.IsValid)return View(model);
        var user=await users.FindByEmailAsync(model.Email.Trim());
        if(user==null)
        {
            ModelState.AddModelError("","Invalid email or password.");
            return View(model);
        }
        var result=await signIn.PasswordSignInAsync(user,model.Password,false,true);
        if(!result.Succeeded)
        {
            ModelState.AddModelError("","Invalid email or password.");
            return View(model);
        }
        return !string.IsNullOrWhiteSpace(model.ReturnUrl)&&Url.IsLocalUrl(model.ReturnUrl)?LocalRedirect(model.ReturnUrl):RedirectToAction("Index","Home");
    }
    [HttpPost,Authorize,ValidateAntiForgeryToken] public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction("Index","Home");
    }
}
