using ControleDeEstoque.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControleDeEstoque.Data {
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            // Configurações adicionais se necessário
        }

        public DbSet<EstoqueItem> EstoqueItens { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<EstoqueOperacao> EstoqueOperacoes { get; set; }
    }
}
