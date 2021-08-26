using System;
using System.Threading.Tasks;
using ASPNET.Business.Interfaces;
using ASPNET.Business.Models;
using ASPNET.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ASPNET.Data.Repository
{
    public class EnderecoRepository : Repository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(MeuDbContext db) : base(db) { }

        public async Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId)
        {
            return await Db.Enderecos
                .FirstOrDefaultAsync(f => f.FornecedorId == fornecedorId);
        }
    }
}