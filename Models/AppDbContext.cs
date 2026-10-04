using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Base_de_dados_Fonoaudiologia.Models
{
    // A classe agora herda de IdentityDbContext para trazer as tabelas de login
public class AppDbContext : IdentityDbContext<Usuario>    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // As suas tabelas originais mantêm-se intactas aqui
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<DocumentoGerado> DocumentosGerados { get; set; }
        public DbSet<DiretrizIA> DiretrizesIA { get; set; }
    }
}