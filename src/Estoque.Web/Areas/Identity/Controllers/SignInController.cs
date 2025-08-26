using Estoque.Domain.Entities;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class SignInController(AuthService authService, AuditLogService auditLogService, UserManager<ApplicationUser> userManager) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, errorMessage) = await authService.SignInAsync(model.Login, model.Senha, model.LembrarMe);

        if (success)
        {
            return RedirectToAction("Index", "Home", new { area = "Estoque" });
        }

        ModelState.AddModelError(string.Empty, errorMessage ?? "Login falhou.");
        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "SignIn", new { area = "Identity" }, protocol: Request.Scheme, host: Request.Host.Value);

        var properties = authService.ConfigureExternalAuthenticationProperties("Google", redirectUrl!);

        return new ChallengeResult("Google", properties);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await authService.ExternalLoginSignInAsync();

        if (result.Succeeded)
            return RedirectToAction("Index", "Home", new { area = "Estoque" });

        ModelState.AddModelError(string.Empty, "Falha ao autenticar com Google.");
        return View("Index");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, [FromServices] EmailSender emailSender)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "SignIn", new { token, email = user.Email }, Request.Scheme);

            var htmlMessage = $"<p>Olá, clique no link abaixo para redefinir sua senha:</p><p><a href='{resetLink}'>Redefinir senha</a></p>";

            await emailSender.SendEmailAsync(user.Email, "Redefinição de senha", htmlMessage);
        }

        ViewBag.Message = "Se o e-mail existir, um link de redefinição foi enviado.";
        return View(model);
    }
}