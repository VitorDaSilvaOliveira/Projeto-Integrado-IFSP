using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Estoque.Tests.E2ETests.PageObjects;
using System;
using System.IO;

namespace Estoque.Tests.E2ETests
{
    public class SeleniumTestBase : IDisposable
    {
        protected IWebDriver Driver { get; private set; }
        protected WebDriverWait Wait { get; private set; }
        protected string BaseUrl { get; set; } = "https://ifspestoque.azurewebsites.net/";

      //  protected string BaseUrl { get; set; } = "https://localhost:7262";

        // Page Objects
        protected LoginPage LoginPage { get; private set; }
        protected UsuarioPage UsuarioPage { get; private set; }

        public SeleniumTestBase()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--start-maximized");

            // ----------------------------------------------------
            // CORREÇÃO CRÍTICA PARA HTTPS/LOCALHOST
            // ----------------------------------------------------
            options.AddArgument("--ignore-certificate-errors"); // Ignora problemas de certificado
            options.AddArgument("--allow-insecure-localhost"); // Permite conexões inseguras no localhost

            // Isso é um fix comum para problemas de tempo limite de carregamento:
            options.AddAdditionalOption("useAutomationExtension", false);
            // -

            Driver = new ChromeDriver(options);
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));

            // Inicializar Page Objects
            LoginPage = new LoginPage(Driver, Wait, BaseUrl);
            UsuarioPage = new UsuarioPage(Driver, Wait, BaseUrl);
        }

        protected void TakeScreenshot(string fileName)
        {
            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            var path = $"Screenshots/{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            Directory.CreateDirectory("Screenshots");
            screenshot.SaveAsFile(path);
        }

        public void Dispose()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
    }
}
