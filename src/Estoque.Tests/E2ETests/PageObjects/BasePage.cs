using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using SeleniumExtras.WaitHelpers; // Certifique-se de ter este using no seu arquivo


namespace Estoque.Tests.E2ETests.PageObjects
{
    public abstract class BasePage
    {
        protected IWebDriver Driver { get; }
        protected WebDriverWait Wait { get; }
        protected string BaseUrl { get; }

        protected BasePage(IWebDriver driver, WebDriverWait wait, string baseUrl)
        {
            Driver = driver;
            Wait = wait;
            BaseUrl = baseUrl;
        }

        // Métodos auxiliares comuns
        protected void NavigateTo(string relativeUrl)
        {
            Driver.Navigate().GoToUrl($"{BaseUrl}{relativeUrl}");
        }

        protected IWebElement WaitForElement(By locator, int timeoutSeconds = 20)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        protected void Click(By locator)
        {
            // WaitForElement(locator).Click();
            var element = WaitForElement(locator, (int)Wait.Timeout.TotalSeconds);
            // Tentativa 1: Submit (Mais robusto para formulários)
            if (element.GetAttribute("type")?.ToLower() == "submit")
            {
                element.Submit();
            }
            else
            {
                // Tentativa 2: Clique normal (para links, etc.)
                try
                {
                    element.Click();
                }
                catch (ElementNotInteractableException)
                {
                    // Tentativa 3: Clique via JavaScript (Último recurso contra bloqueios)
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
                }
            }

            // element.Click();
        }

        protected void Type(By locator, string text)
        {
            var element = WaitForElement(locator);
            element.Clear();
            element.SendKeys(text);
        }

        protected string GetText(By locator)
        {
            return WaitForElement(locator).Text;
        }

        protected bool IsElementPresent(By locator)
        {
            try
            {
                Driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected bool IsElementVisible(By locator)
        {
            try
            {
                return Driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected void WaitForPageLoad()
        {
            Wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
