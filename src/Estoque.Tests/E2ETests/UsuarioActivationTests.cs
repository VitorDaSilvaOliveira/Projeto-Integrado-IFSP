using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using FluentAssertions;
using Xunit;

namespace Estoque.Tests.E2ETests
{
    [Collection("Selenium")]
    public class LoginTests : SeleniumTestBase, IDisposable
    {
        private readonly WebDriverWait wait;

        public LoginTests()
        {
            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
        }

        // Método reutilizável para tentar login e validar sucesso/fracasso
        private void TentaLogin(string login, string senha, bool esperaSucesso)
        {
            LoginPage.GoToLoginPage();

            // Preenche campos
            Driver.FindElement(By.Id("Login")).Clear();
            Driver.FindElement(By.Id("Login")).SendKeys(login);

            Driver.FindElement(By.Id("senha")).Clear();
            Driver.FindElement(By.Id("senha")).SendKeys(senha);

            // Encontra botão e simula hover + clique
            var botaoEntrar = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("button.btn.BGC-azul-2")));

            new Actions(Driver).MoveToElement(botaoEntrar).Perform();
            System.Threading.Thread.Sleep(500); // hover visível

            botaoEntrar.Click();

            if (esperaSucesso)
            {
                // Espera URL mudar, indicando login OK
                wait.Until(d => !d.Url.Contains("/Identity/SignIn"));
                Driver.Url.Should().NotContain("/Identity/SignIn", "Usuário ativo deveria conseguir fazer login");
            }
            else
            {
                // Espera permanecer na página de login e mensagem de erro aparecer
                wait.Until(d => d.Url.Contains("/Identity/SignIn"));
                wait.Until(d => d.FindElements(By.CssSelector(".text-danger, .alert-danger")).Count > 0);

                var mensagensErro = Driver.FindElements(By.CssSelector(".text-danger, .alert-danger"));
                mensagensErro.Should().NotBeEmpty("Deveria exibir mensagem de erro para usuário bloqueado ou inválido");
            }
        }

        [Fact]
        [Trait("E2E", "Login")]
        public void UsuarioAtivo_DeveConseguirFazerLogin()
        {
            try
            {
                TentaLogin("Admin", "Admin@123", esperaSucesso: true);
                TakeScreenshot("Usuario_Ativo_Login_Sucesso");
            }
            catch (Exception)
            {
                TakeScreenshot("ERRO_Usuario_Ativo");
                throw;
            }
        }

        [Fact]
        [Trait("E2E", "Login")]
        public void UsuarioBloqueado_NaoDeveConseguirFazerLogin()
        {
            try
            {
                TentaLogin("Funcionário", "Mario@123", esperaSucesso: false);
                TakeScreenshot("Usuario_Bloqueado_Login_Negado");
            }
            catch (Exception)
            {
                TakeScreenshot("ERRO_Usuario_Bloqueado");
                throw;
            }
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
