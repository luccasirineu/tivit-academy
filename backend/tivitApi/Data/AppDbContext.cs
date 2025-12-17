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
        public DbSet<Documentos> Documentos { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Evento> Eventos{ get; set; }
        public DbSet<Conteudo> Conteudos { get; set; }
        public DbSet<Materia> Materias { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Aluno>()
                .HasOne(a => a.Matricula)
                .WithOne()
                .HasForeignKey<Aluno>(a => a.MatriculaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.curso)
                .WithMany()
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conteudo>()
                .HasOne(c => c.Professor)
                .WithMany()
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
