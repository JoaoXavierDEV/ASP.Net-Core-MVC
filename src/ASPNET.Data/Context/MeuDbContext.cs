using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASPNET.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPNET.Data.Context {
    public class MeuDbContext : DbContext{
        public MeuDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }

    }
}
