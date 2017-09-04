using CantinaSimples.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaSimples.Web.Services
{
    /// <summary>
    /// Operações relacionadas ao estoque. Os métodos dessa classe não criam ou encerram transações,
    /// o que é de responsabilidade do utilizador da classe.
    /// </summary>
    public class EstoqueService
    {
        private CantinaContext Context;

        public EstoqueService(CantinaContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Adiciona o movimento no contexto e atualiza o saldo do produto.
        /// </summary>
        /// <param name="movimento">Movimento que está sendo criado.</param>
        /// <returns>O movimento que foi criado.</returns>
        public MovimentoEstoque CriarMovimento(MovimentoEstoque movimento)
        {
            Context.MovimentosEstoque.Add(movimento);
            movimento.Produto.Saldo += movimento.Quantidade;
            Context.Entry(movimento.Produto).State = EntityState.Modified;
            return movimento;
        }

        /// <summary>
        /// Cria um movimento de estoque a partir do item de uma venda. O item deve pertencer ao contexto.
        /// </summary>
        /// <param name="itemVenda">O item da venda a partir do qual será criado o movimento de estoque. Deve pertencer ao contexto.</param>
        /// <returns>O movimento de estoque criado.</returns>
        public MovimentoEstoque CriarMovimento(ItemVenda itemVenda)
        {
            MovimentoEstoque movimento = new MovimentoEstoque
            {
                Produto = itemVenda.Produto,
                Quantidade = itemVenda.Quantidade * (-1)
            };
            itemVenda.Produto.MovimentosEstoque.Add(movimento);
            itemVenda.MovimentoEstoque = movimento;
            return CriarMovimento(movimento);
        }

        /// <summary>
        /// Atualiza o saldo do produto do movimento com base nos novos valores.
        /// </summary>
        /// <param name="movimento">Dados que serão atualizados. Deve pertencer ao contexto.</param>
        /// <param name="quantidadeAnterior">Quantidade anterior do movimento de estoque para adicionar a diferença no saldo do produto.</param>
        /// <returns>O movimento de estoque que foi alterado.</returns>
        public MovimentoEstoque EditarMovimento(MovimentoEstoque movimento, int quantidadeAnterior)
        {
            int quantidadeNova = movimento.Quantidade;
            int diferenca = quantidadeNova - quantidadeAnterior;
            int saldoAnterior = movimento.Produto.Saldo;
            int saldoNovo = saldoAnterior + diferenca;

            movimento.Produto.Saldo = saldoNovo;
            return movimento;
        }

        /// <summary>
        /// Remove o movimento de estoque e atualiza o saldo do produto.
        /// </summary>
        /// <param name="movimento">O movimento que está sendo removido. Deve pertencer ao contexto.</param>
        /// <returns>Movimento de estoque que foi removido.</returns>
        public MovimentoEstoque RemoverMovimento(MovimentoEstoque movimento)
        {
            movimento.Produto.Saldo -= movimento.Quantidade;
            Context.MovimentosEstoque.Remove(movimento);
            return movimento;
        }
    }
}
