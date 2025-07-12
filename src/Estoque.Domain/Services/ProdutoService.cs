// Local: src/Estoque.Domain/Services/ProdutoService.cs
using Estoque.Domain.Entities;
using System;

namespace Estoque.Domain.Services
{
    public class ProdutoService
    {
        public Produto CriarNovoProduto(string nome, string descricao, decimal preco, int quantidade)
        {
            // --- Regras de Negócio e Validações ---
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("O nome do produto é obrigatório.");
            }

            if (preco < 0)
            {
                throw new ArgumentException("O preço do produto não pode ser negativo.");
            }

            if (quantidade < 0)
            {
                throw new ArgumentException("A quantidade em estoque não pode ser negativa.");
            }

            // Se todas as validações passaram, cria o objeto Produto.
            var novoProduto = new Produto
            {
                Nome = nome,
                Descricao = descricao,
                Preco = preco,
                QuantidadeEstoque = quantidade,
                EstoqueMinimo = 10 
            };

            return novoProduto;
        }
    }
}