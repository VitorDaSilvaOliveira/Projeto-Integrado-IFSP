using System.Security.Claims;
using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController(UserManager<ApplicationUser> userManager, UserService userService,SignInManager<ApplicationUser> signInManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var formViewUserAsync = await userService.GetFormViewUserAsync();
        var resultGridUserAsync = await formViewUserAsync.GetResultAsync();

        if (resultGridUserAsync is IActionResult actionResult)
            return actionResult;

        ViewBag.FormViewUser = resultGridUserAsync.Content;
        return View();
    }
    
    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser(model.FirstName, model.LastName)
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var roleResult = await userManager.AddToRoleAsync(user, "Guest");

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            TempData["Success"] = "Usuário criado com sucesso!";
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> UserDetails(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return NotFound();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AvatarUrl = user.AvatarFileName,
            Status = user.Status
        };

        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> UserDetails(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await userManager.FindByIdAsync(model.Id.ToString());

        if (user == null)
            return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        TempData["Success"] = "Informações do usuário atualizadas com sucesso!";
        return RedirectToAction("UserDetails", new { userId = user.Id });
    }
    
    [HttpGet]
    public async Task<IActionResult> Impersonate(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return NotFound();

        HttpContext.Session.SetString("OriginalUserId", User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        await signInManager.CreateUserPrincipalAsync(user);
        
        await signInManager.SignOutAsync(); 
        await signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Home", new { area = "Estoque" });
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return NotFound();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AvatarUrl = user.AvatarFileName
        };

        return View(model);
    }
}