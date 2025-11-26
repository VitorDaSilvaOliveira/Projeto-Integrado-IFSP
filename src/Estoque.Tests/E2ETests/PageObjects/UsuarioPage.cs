using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Estoque.Tests.E2ETests.PageObjects
{
    public class UsuarioPage : BasePage
    {
        // Locators
        private By TabelaUsuarios => By.CssSelector("table");
        private By LinhasTabela => By.CssSelector("table tbody tr");
        private By FiltroStatus => By.Id("filtroStatus");
        private By ModalConfirmacao => By.Id("confirmModal");
        private By BotaoConfirmarModal => By.CssSelector("button[data-confirm='yes']");
        private By MensagemSucesso => By.CssSelector(".alert-success");
        private By MensagemErro => By.CssSelector(".alert-danger");

        // Construtor
        public UsuarioPage(IWebDriver driver, WebDriverWait wait, string baseUrl)
            : base(driver, wait, baseUrl)
        {
        }

        // Navegação
        public UsuarioPage GoToUsuarioPage()
        {
            NavigateTo("/Usuario");
            WaitForElement(TabelaUsuarios);
            return this;
        }

        // Ações
        public UsuarioPage DesativarUsuario(string username)
        {
            var row = FindUserRow(username);
            if (row == null)
                throw new NoSuchElementException($"Usuário '{username}' não encontrado");

            var deactivateButton = row.FindElement(By.CssSelector("button[data-action='deactivate']"));
            deactivateButton.Click();

            ConfirmarAcao();
            Thread.Sleep(1000); // Aguardar processamento

            return this;
        }

        public UsuarioPage AtivarUsuario(string username)
        {
            var row = FindUserRow(username);
            if (row == null)
                throw new NoSuchElementException($"Usuário '{username}' não encontrado");

            var activateButton = row.FindElement(By.CssSelector("button[data-action='activate']"));
            activateButton.Click();

            ConfirmarAcao();
            Thread.Sleep(1000);

            return this;
        }

        public UsuarioPage FiltrarPorStatus(string status)
        {
            var filtro = WaitForElement(FiltroStatus);
            var select = new SelectElement(filtro);
            select.SelectByValue(status);
            Thread.Sleep(500); // Aguardar filtro aplicar
            return this;
        }

        public UsuarioPage RefreshPage()
        {
            Driver.Navigate().Refresh();
            WaitForElement(TabelaUsuarios);
            return this;
        }

        // Verificações
        public string GetStatusUsuario(string username)
        {
            var row = FindUserRow(username);
            if (row == null)
                throw new NoSuchElementException($"Usuário '{username}' não encontrado");

            return row.FindElement(By.CssSelector("td[data-status]")).Text;
        }

        public bool UsuarioExiste(string username)
        {
            return FindUserRow(username) != null;
        }

        public bool BotaoDesativarEstaDesabilitado(string username)
        {
            var row = FindUserRow(username);
            if (row == null)
                return false;

            var buttons = row.FindElements(By.CssSelector("button[data-action='deactivate']"));

            if (buttons.Count == 0)
                return true; // Botão não existe = desabilitado

            var button = buttons[0];
            return button.GetAttribute("disabled") != null ||
                   button.GetAttribute("class").Contains("disabled");
        }

        public bool MensagemSucessoExibida()
        {
            return IsElementVisible(MensagemSucesso);
        }

        public string GetMensagemSucesso()
        {
            return GetText(MensagemSucesso);
        }

        public bool MensagemErroExibida()
        {
            return IsElementVisible(MensagemErro);
        }

        public List<string> GetTodosUsuarios()
        {
            var rows = Driver.FindElements(LinhasTabela);
            return rows.Select(r => r.FindElement(By.CssSelector("td:first-child")).Text).ToList();
        }

        public int GetQuantidadeUsuarios()
        {
            return Driver.FindElements(LinhasTabela).Count;
        }

        // Métodos privados auxiliares
        private void ConfirmarAcao()
        {
            WaitForElement(ModalConfirmacao);
            Click(BotaoConfirmarModal);
        }

        private IWebElement FindUserRow(string username)
        {
            var rows = Driver.FindElements(LinhasTabela);

            foreach (var row in rows)
            {
                if (row.Text.Contains(username))
                {
                    return row;
                }
            }

            return null;
        }
    }
}
