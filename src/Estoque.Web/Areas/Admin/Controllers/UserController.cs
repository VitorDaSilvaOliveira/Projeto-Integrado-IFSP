using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController(UserManager<ApplicationUser> userManager, UserService userService) : Controller
{
    public async Task<IActionResult> User()
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
            UserName = model.Username,
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
            return RedirectToAction("User");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }
}