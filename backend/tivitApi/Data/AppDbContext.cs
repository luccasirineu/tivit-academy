using Microsoft.EntityFrameworkCore;
using tivitApi.Models;

namespace tivitApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Professor> Professores { get; set; }
        public DbSet<ComprovantePagamento> ComprovantesPagamento { get; set; }

    }
}
