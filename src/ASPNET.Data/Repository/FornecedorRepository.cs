using System;
using System.Threading.Tasks;
using ASPNET.Business.Interfaces;
using ASPNET.Business.Models;
using ASPNET.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ASPNET.Data.Repository
{
    public class FornecedorRepository : Repository<Fornecedor> , IFornecedorRepository

    {
        public FornecedorRepository(MeuDbContext context) : base(context) { }

        public async Task<Fornecedor> ObterFornecedorEndereco(Guid id)
        {
            return await Db.Fornecedores
                .AsNoTracking()
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Fornecedor> ObterFornecedorProdutoEndereco(Guid id)
        {
            return await Db.Fornecedores.AsNoTracking()
                .Include(c => c.Produtos)
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}