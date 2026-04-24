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
        public DbSet<ConteudoContexto> ConteudosContexto { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<Chamada> Chamadas { get; set; }
        public DbSet<Turma> Turmas { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<NotificacaoTurma> NotificacoesTurmas { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar enums para serem armazenados como STRING no banco
            modelBuilder.Entity<Matricula>()
                .Property(m => m.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Curso>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Aluno>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Professor>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Administrador>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Nota>()
                .Property(n => n.Status)
                .HasConversion<string>();

            // Relacionamentos existentes
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

            modelBuilder.Entity<ConteudoContexto>()
                .HasOne(cc => cc.Conteudo)
                .WithMany()
                .HasForeignKey(cc => cc.ConteudoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Nota>()
                .HasIndex(n => new { n.AlunoId, n.MateriaId })
                .IsUnique();

            modelBuilder.Entity<Chamada>()
                .HasOne(c => c.Turma)
                .WithMany()
                .HasForeignKey(c => c.TurmaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Chamada>()
                .HasOne(c => c.Matricula)
                .WithMany()
                .HasForeignKey(c => c.MatriculaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<NotificacaoTurma>()
                .HasKey(nt => new { nt.NotificacaoId, nt.TurmaId });

            modelBuilder.Entity<NotificacaoTurma>()
                .HasOne(nt => nt.Notificacao)
                .WithMany(n => n.NotificacaoTurmas)
                .HasForeignKey(nt => nt.NotificacaoId);

            modelBuilder.Entity<NotificacaoTurma>()
                .HasOne(nt => nt.Turma)
                .WithMany(t => t.NotificacaoTurmas)
                .HasForeignKey(nt => nt.TurmaId);


        }

    }
}
