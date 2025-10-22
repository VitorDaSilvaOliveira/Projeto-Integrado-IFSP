using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using SeleniumExtras.WaitHelpers; 
using OpenQA.Selenium.Support.Extensions;

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
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            //return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }
        protected void ExecuteScript(string script, IWebElement element)
        {
            ((OpenQA.Selenium.IJavaScriptExecutor)Driver).ExecuteScript(script, element);
        }
        protected void Click(By locator)
        {
            var element = WaitForElement(locator, (int)Wait.Timeout.TotalSeconds);

            try
            {
                element.Click();
            }
            catch (OpenQA.Selenium.ElementClickInterceptedException)
            {
                ExecuteScript("arguments[0].click();", element);
            }
            catch (OpenQA.Selenium.ElementNotInteractableException)
            {
                ExecuteScript("arguments[0].click();", element);
            }
            catch (OpenQA.Selenium.WebDriverException ex) when (ex.Message.Contains("not clickable"))
            {
                ExecuteScript("arguments[0].click();", element);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Botão 'Entrar' não estava disponível no tempo esperado.");
            }


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
