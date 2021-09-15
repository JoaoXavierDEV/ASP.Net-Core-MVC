using System;
using System.Threading.Tasks;
using ASPNET.Business.Models;

namespace ASPNET.Business.Interfaces
{
    public interface IProdutoService : IDisposable
    {
        Task Adicionar(Produto produto);
        Task Atualizar(Produto produto);
        Task Remover(Guid id);
    }
}