using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Estoque.Domain.Models;
using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class SignInController(AuthService authService, UserManager<ApplicationUser> userManager) : Controller
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

        var user = await userManager.FindByNameAsync(model.Login);
        
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Login ou senha inválidos.");
            return View(model);
        }

        if (user.Status == UserStatus.Inativo)
        {
            ModelState.AddModelError(string.Empty, "Login ou senha inválidos.");
            return View(model);
        }

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

            var htmlMessage = $"""
                               <html>
                               <body style='font-family: Arial, sans-serif; font-size: 14px; color: #333; line-height: 1.5;'>
                                   <p>Olá,</p>
                                   <p>Clique no botão abaixo para redefinir sua senha:</p>
                                   <p style='text-align: right; margin: 30px 0;'>
                                       <a href='{System.Net.WebUtility.HtmlEncode(resetLink)}'
                                          style='display: inline-block; padding: 12px 25px; color: #333; 
                                                 background-color: #ffc107; font-weight: bold; text-decoration: none; 
                                                 border-radius: 6px; border: 1px solid #e0a800;'>
                                           Redefinir senha
                                       </a>
                                   </p>
                                   <p>Se você não solicitou essa ação, pode ignorar este e-mail.</p>
                               </body>
                               </html>
                               """;
            
            await emailSender.SendEmailAsync(user.Email, "Vip Penha - Redefinição de Senha", htmlMessage);
        }

        TempData["Success"] = "Se o e-mail existir, um link de redefinição foi enviado.";
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
            return BadRequest("Token ou e-mail inválido.");

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            TempData["Success"] = "Se o e-mail for válido, sua senha foi redefinida.";
            return View("ResetPassword");
        }

        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            TempData["Success"] = "Senha redefinida com sucesso! Agora você já pode fazer login.";
            return View("ResetPassword");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }
}