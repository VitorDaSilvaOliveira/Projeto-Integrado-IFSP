using Estoque.Tests.E2ETests.PageObjects;
using FluentAssertions;
using Xunit;

namespace Estoque.Tests.E2ETests
{
    [Collection("Selenium")]
    public class UsuarioActivationTests : SeleniumTestBase
    {
        [Fact]
        [Trait("E2E", "Login")]
        public void DeveRealizarLoginComSucesso()
        {
            try
            {
                // ARRANGE & ACT
                LoginPage.GoToLoginPage()
                         .Login("Admin", "Admin@123");  // ✅ Ajuste as credenciais

                // ASSERT
                LoginPage.IsLoginSuccessful().Should().BeTrue("Login deveria ter sido realizado");

                // Verificar redirecionamento
                Driver.Url.Should().NotContain("/Identity/SignIn", "Deveria ter saído da página de login");

                TakeScreenshot("Login_Sucesso");
            }
            catch (Exception ex)
            {
                TakeScreenshot("Erro_Login");
                throw;
            }
        }




    }
}
