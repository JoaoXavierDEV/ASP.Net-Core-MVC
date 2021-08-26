using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNET.Business.Interfaces;
using ASPNET.Business.Models;
using ASPNET.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ASPNET.Data.Repository {
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {

        public ProdutoRepository(MeuDbContext context) : base(context) {}

        public async Task<Produto> ObterProdutoFornecedor(Guid id) {
            return await Db.Produtos.AsNoTracking().Include(f => f.Fornecedor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Produto>> ObterFornecedores() {
            return await Db.Produtos.AsNoTracking().Include(f => f.Fornecedor)
                .OrderBy(p => p.Nome).ToListAsync();
        }
        public async Task<IEnumerable<Produto>> ObterProdutosPorFornecedor(Guid id) {
            return await Buscar(p => p.FornecedorId == id);

        }



    }
}