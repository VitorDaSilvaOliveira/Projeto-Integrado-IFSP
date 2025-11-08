using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Xunit;

public class LoginButtonTest : IDisposable
{
    private IWebDriver driver;
    private WebDriverWait wait;

    public LoginButtonTest()
    {
        var options = new ChromeOptions();
        options.AcceptInsecureCertificates = true; // se usar https local
        driver = new ChromeDriver(options);
        driver.Manage().Window.Maximize();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
    }

    [Fact]
    public void TestaBotaoEntrarComHover()
    {
        driver.Navigate().GoToUrl("https://localhost:7262/Identity/SignIn");

        var botaoEntrar = wait.Until(
            SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(
                By.CssSelector("button.btn.BGC-azul-2"))
        );

        Actions actions = new Actions(driver);
        actions.MoveToElement(botaoEntrar).Perform();

        System.Threading.Thread.Sleep(500); // deixa o hover "assentar"

        botaoEntrar.Click();

        // Assert.False(driver.Url.Contains("/Identity/SignIn")); // se espera sair da página de login
    }

    public void Dispose()
    {
        driver.Quit();
        driver.Dispose();
    }
}
