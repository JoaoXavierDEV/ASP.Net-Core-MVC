using ASPNET.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASPNET.Data.Mappings {
    class ProdutoMapping : IEntityTypeConfiguration<Produto> {
        public void Configure(EntityTypeBuilder<Produto> builder) {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(p => p.Descricao)
                .IsRequired()
                .HasColumnType("varchar(1000)");

            builder.Property(p => p.Imagem)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.ToTable("Produtos");
        }


    }
    class FornecedorMapping : IEntityTypeConfiguration<Fornecedor> {
        public void Configure(EntityTypeBuilder<Fornecedor> builder) {

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(p => p.Documento)
                .IsRequired()
                .HasColumnType("varchar(14)");
 
            // 1 : 1 => Fornecedor : Endereco

            builder.HasOne(f => f.Endereco)
                .WithOne(e => e.Fornecedor);

            // 1 : N => Fornecedor : Produtos

            builder.HasMany(f => f.Produtos)
                .WithOne(p => p.Fornecedor)
                .HasForeignKey(p => p.FornecedorId);


            builder.ToTable("Fornecedores");
        }


    }
    class EnderecoMapping : IEntityTypeConfiguration<Endereco> {
        public void Configure(EntityTypeBuilder<Endereco> builder) {
            builder.HasKey(p => p.Id);

            builder.Property(c => c.Logradouro)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.Numero)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(c => c.Cep)
                .IsRequired()
                .HasColumnType("varchar(8)");

            builder.Property(c => c.Complemento)
                .IsRequired()
                .HasColumnType("varchar(250)");

            builder.Property(c => c.Cidade)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.Bairro)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.Estado)
                .IsRequired()
                .HasColumnType("varchar(50)");


            builder.ToTable("Enderecos");
        }


    }
}
