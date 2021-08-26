using System;
using System.Threading.Tasks;
using ASPNET.Business.Models;

namespace ASPNET.Business.Interfaces
{
    public interface IEnderecoRepository : IRepository<Endereco>

    {
        Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId);
    }
}