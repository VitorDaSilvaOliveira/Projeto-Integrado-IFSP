using Estoque.Infrastructure.Utils;
using Xunit;
using System;

namespace Estoque.Tests.UnitTests
{
    // #################################################################
    // Testes da Classe de Lógica: ObjectUtils (8 Testes)
    // #################################################################
    public class ObjectUtilsTests
    {
        private readonly object _testeObj = new
        {
            Nome = "Teste",
            Id = 10,
            Valor = 123.45m,
            ValorDouble = 50.75,
            Data = (DateTime?)new DateTime(2025, 1, 1)
        };

        // --- SafeGetString ---

        [Fact]
        public void SafeGetString_Should_Return_String_When_Property_Exists()
        {
            var result = ObjectUtils.SafeGetString(_testeObj, "Nome");
            Assert.Equal("Teste", result);
        }

        [Fact]
        public void SafeGetString_Should_Return_String_For_Non_String_Property()
        {
            // Testa a conversão (ToString())
            var result = ObjectUtils.SafeGetString(_testeObj, "Id");
            Assert.Equal("10", result);
        }

        [Fact]
        public void SafeGetString_Should_Return_Null_When_Property_Not_Exists()
        {
            var result = ObjectUtils.SafeGetString(_testeObj, "PropriedadeInexistente");
            Assert.Null(result);
        }

        // --- SafeGetInt ---

        [Fact]
        public void SafeGetInt_Should_Return_Int_When_Property_Exists()
        {
            var result = ObjectUtils.SafeGetInt(_testeObj, "Id");
            Assert.Equal(10, result);
        }

        [Fact]
        public void SafeGetInt_Should_Return_Zero_When_Property_Not_Exists()
        {
            var result = ObjectUtils.SafeGetInt(_testeObj, "PropriedadeInexistente");
            Assert.Equal(0, result);
        }

        // --- SafeGetDecimal ---

        [Fact]
        public void SafeGetDecimal_Should_Return_Decimal_When_Property_Exists()
        {
            var result = ObjectUtils.SafeGetDecimal(_testeObj, "Valor");
            Assert.Equal(123.45m, result);
        }

        [Fact]
        public void SafeGetDecimal_Should_Convert_Double_To_Decimal()
        {
            var result = ObjectUtils.SafeGetDecimal(_testeObj, "ValorDouble");
            Assert.Equal(50.75m, result);
        }

        // --- SafeGetObject ---

        [Fact]
        public void SafeGetObject_Should_Return_Object_When_Property_Exists()
        {
            var result = ObjectUtils.SafeGetObject(_testeObj, "Data");
            Assert.Equal(new DateTime(2025, 1, 1), result);
        }
    }
}