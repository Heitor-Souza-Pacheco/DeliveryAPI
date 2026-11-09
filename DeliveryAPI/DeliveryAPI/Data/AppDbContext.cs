using DeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<ItemCarrinho> ItensCarrinho => Set<ItemCarrinho>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Nome).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(254).IsRequired();
            entity.Property(u => u.Senha).HasMaxLength(100).IsRequired();
            entity.Property(u => u.TipoConta).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("produtos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nome).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Descricao).HasMaxLength(500);
            entity.Property(p => p.Preco).HasPrecision(10, 2);
            entity.HasData(
                new Produto { Id = 1, Nome = "Rosquinha Tradicional", Descricao = "Rosquinha clássica com açúcar.", Preco = 3.50m, Disponivel = true },
                new Produto { Id = 2, Nome = "Rosquinha de Chocolate", Descricao = "Rosquinha coberta com chocolate.", Preco = 5.00m, Disponivel = true },
                new Produto { Id = 3, Nome = "Rosquinha de Doce de Leite", Descricao = "Rosquinha com cobertura de doce de leite.", Preco = 5.50m, Disponivel = true },
                new Produto { Id = 4, Nome = "Rosquinha de Coco", Descricao = "Rosquinha com cobertura de coco.", Preco = 4.50m, Disponivel = true },
                new Produto { Id = 5, Nome = "Rosquinha de Morango", Descricao = "Rosquinha com cobertura sabor morango.", Preco = 5.00m, Disponivel = true }
            );
        });

        modelBuilder.Entity<ItemCarrinho>(entity =>
        {
            entity.ToTable("itens_carrinho");
            entity.HasKey(i => new { i.UsuarioId, i.ProdutoId });
            entity.HasOne<Usuario>().WithMany().HasForeignKey(i => i.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Produto>().WithMany().HasForeignKey(i => i.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("pedidos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Status).HasMaxLength(30).IsRequired();
            entity.HasOne<Usuario>().WithMany().HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Restrict);
            entity.Ignore(p => p.Total);
        });

        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.ToTable("itens_pedido");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.NomeProduto).HasMaxLength(100).IsRequired();
            entity.Property(i => i.PrecoUnitario).HasPrecision(10, 2);
            entity.Ignore(i => i.Subtotal);
            entity.HasOne<Pedido>().WithMany(p => p.Itens).HasForeignKey(i => i.PedidoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Produto>().WithMany().HasForeignKey(i => i.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
