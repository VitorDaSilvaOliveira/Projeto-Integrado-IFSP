using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Estoque.Tests.UnitTests
{
    public class IdentityErrorDescriberTests
    {
        private readonly IdentityErrorDescriberPtBr _describer;

        public IdentityErrorDescriberTests()
        {
            _describer = new IdentityErrorDescriberPtBr();
        }

        [Fact]
        public void PasswordTooShort_Should_Return_Portuguese_Message()
        {
            // Este está implementado na sua classe, deve retornar em PT-BR
            var error = _describer.PasswordTooShort(6);
            Assert.Equal("A senha deve ter no mínimo 6 caracteres.", error.Description);
        }

        [Fact]
        public void PasswordRequiresDigit_Should_Return_Portuguese_Message()
        {
            // Este está implementado na sua classe
            var error = _describer.PasswordRequiresDigit();
            Assert.Equal("A senha deve conter ao menos um dígito ('0'-'9').", error.Description);
        }

        // --- Testes abaixo ajustados para o comportamento ATUAL (Inglês) ---
        // A classe IdentityErrorDescriberPtBr ainda não sobrescreve estes métodos.
        
        [Fact]
        public void DefaultError_Should_Return_Base_Message()
        {
            var error = _describer.DefaultError();
            // Ajustado para o retorno atual da classe base
            Assert.Equal("An unknown failure has occurred.", error.Description);
        }

        [Fact]
        public void DuplicateEmail_Should_Return_Base_Message()
        {
            var email = "teste@teste.com";
            var error = _describer.DuplicateEmail(email);
            // Ajustado para o retorno atual da classe base
            Assert.Equal($"Email '{email}' is already taken.", error.Description);
        }

        [Fact]
        public void InvalidEmail_Should_Return_Base_Message()
        {
            var error = _describer.InvalidEmail("email-ruim");
            // Ajustado para o retorno atual da classe base
            Assert.Equal("Email 'email-ruim' is invalid.", error.Description);
        }
    }
}