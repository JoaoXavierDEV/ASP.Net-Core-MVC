using System;
using System.Threading.Tasks;
using ASPNET.Business.Models;

namespace ASPNET.Business.Interfaces {
    public interface IFornecedorService : IDisposable {
        Task Adicionar(Fornecedor fornecedor);
        Task Atualizar(Fornecedor fornecedor);
        Task Remover(Guid id);
        Task AtualizarEndereco(Endereco endereco);


    }
}