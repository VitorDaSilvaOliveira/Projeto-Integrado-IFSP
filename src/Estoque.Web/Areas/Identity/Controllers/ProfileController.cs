using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Estoque.Infrastructure.Data;
using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class ProfileController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IWebHostEnvironment environment,
    EstoqueDbContext context,
    UserService userService)
    : Controller
{
    public IActionResult Index()
    {
        var userId = userManager.GetUserId(User);
        var user = context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) return NotFound();

        var model = new PersonalInfoViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AvatarUrl = string.IsNullOrEmpty(user.AvatarFileName)
                ? "/img/default_profile.png"
                : $"/uploads/avatars/{user.AvatarFileName}"
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SavePersonalInformation(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            ModelState.AddModelError("", "Nome e sobrenome são obrigatórios.");
            return RedirectToAction("Index");
        }

        var userId = userManager.GetUserId(User);
        var user = context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null) return NotFound();

        user.FirstName = firstName.Trim();
        user.LastName = lastName.Trim();

        context.Update(user);
        await context.SaveChangesAsync();

        await signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "Informações salvas com sucesso!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SaveProfilePicture(IFormFile? newProfilePicture)
    {
        if (newProfilePicture == null || newProfilePicture.Length == 0)
            return Json(new { success = false, message = "Arquivo inválido." });

        var userId = userManager.GetUserId(User);
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return Json(new { success = false, message = "Usuário não encontrado." });

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsFolder);

        var fileExt = Path.GetExtension(newProfilePicture.FileName);
        var fileName = $"{userId}{fileExt}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await newProfilePicture.CopyToAsync(stream);

        user.AvatarFileName = fileName;
        context.Update(user);
        await context.SaveChangesAsync();

        var avatarUrl = Url.Content($"~/uploads/avatars/{fileName}");
        return Json(new { success = true, avatarUrl });
    }

    [HttpGet]
    public IActionResult Avatar(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return NotFound();

        var avatarBytes = userService.GetUserAvatarBytes(userId);

        return File(avatarBytes, "image/png");
    }
}