using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Estoque.Tests.E2ETests.PageObjects
{
    public class LoginPage : BasePage
    {
        private By LoginInput => By.Id("Login");                    // Campo de login
        private By SenhaInput => By.Id("senha");                    // Campo de senha
        private By EntrarButton => By.CssSelector("button[type='submit']");  // Botão entrar
        private By ErrorMessage => By.CssSelector(".text-danger");  
        private By LoginForm => By.CssSelector("form[action='/Identity/SignIn']"); 

        public LoginPage(IWebDriver driver, WebDriverWait wait, string baseUrl)
            : base(driver, wait, baseUrl)
        {
        }



        // Ações
        public LoginPage GoToLoginPage()
        {
            NavigateTo("/Identity/SignIn");  
            WaitForElement(LoginForm);
            return this;
        }

        public LoginPage EnterLogin(string login)
        {
            Type(LoginInput, login);
            return this;
        }

        public LoginPage EnterSenha(string senha)
        {
            Type(SenhaInput, senha);
            return this;
        }

        public void ClickEntrarButton()
        {
            Click(EntrarButton);
            WaitForPageLoad();
        }

        public void Login(string login, string senha)
        {
            EnterLogin(login);
            EnterSenha(senha);
            ClickEntrarButton();
        }

        // Verificações
        public bool IsErrorMessageDisplayed()
        {
            return IsElementVisible(ErrorMessage);
        }

        public string GetErrorMessage()
        {
            return GetText(ErrorMessage);
        }

        public bool IsLoginSuccessful()
        {
            return !Driver.Url.Contains("/Identity/SignIn");
        }

       

        public void Logout()
        {
            try
            {
                var userDropdown = Wait.Until(d =>
                    d.FindElement(By.CssSelector(".dropdown-toggle"))
                );
                userDropdown.Click();

                System.Threading.Thread.Sleep(500);

                var logoutButton = Wait.Until(d =>
                    d.FindElement(By.XPath("//button[contains(text(), 'Sair')]"))
                );
                logoutButton.Click();
            }
            catch
            {
                Driver.Navigate().GoToUrl($"{BaseUrl}/Identity/SignOut");
            }
        }
    }
}
