using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Estoque.Tests.E2ETests.PageObjects
{
    public class LoginPage : BasePage
    {
        // ✅ Seletores Corretos baseados no HTML real
        private By LoginInput => By.Id("Login");                    // Campo de login
        private By SenhaInput => By.Id("senha");                    // Campo de senha
        private By EntrarButton => By.CssSelector("button[type='submit']");  // Botão entrar
        private By ErrorMessage => By.CssSelector(".text-danger");  // Mensagem de erro
        private By LoginForm => By.CssSelector("form[method='post']");  // Formulário

        public LoginPage(IWebDriver driver, WebDriverWait wait, string baseUrl)
            : base(driver, wait, baseUrl)
        {
        }

        // Ações
        public LoginPage GoToLoginPage()
        {
                NavigateTo("/Account/Login");
            Wait.Until(ExpectedConditions.UrlContains("/Login"));
            //Wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.UrlContains("/Login"));


            //NavigateTo("/Identity/SignIn");  // ✅ Rota correta
            WaitForElement(LoginForm,20);
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
           // 1.Espera o botão ficar visível(para garantir que a página carregou)
    var submitButton = WaitForElement(EntrarButton, (int)Wait.Timeout.TotalSeconds);

            // 2. CORREÇÃO CRÍTICA: Encontra o elemento FORMULÁRIO e executa o Submit()

            // Tentativa A: Submeter o formulário pai do botão
            // Esta é a forma mais robusta de ignorar bloqueios de interatividade
            try
            {
                // Encontra o botão e depois navega para o elemento <form> pai
                var formElement = submitButton.FindElement(By.XPath("./ancestor::form[1]"));
                formElement.Submit();
            }
            catch (NoSuchElementException)
            {
                // Se o XPath for muito complexo, submete o próprio botão como fallback (o que já falhou antes)
                submitButton.Submit();
            }

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
            // Verificar se foi redirecionado (não está mais na página de login)
            return !Driver.Url.Contains("/Identity/SignIn");
        }
    }
}
