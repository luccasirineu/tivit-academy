using FluentAssertions;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Exceptions;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class TurmaServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TurmaService _service;

        public TurmaServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _service = new TurmaService(_context);
        }

        [Fact]
        public async Task CriarTurma_DeveCriarTurmaComSucesso()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var dto = new TurmaDTORequest
            {
                Nome = "Turma A",
                CursoId = curso.Id,
                Status = "ATIVO"
            };

            // Act
            await _service.CriarTurma(dto);

            // Assert
            var turmas = await _service.GetAllTurmas();
            turmas.Should().HaveCount(1);
            turmas[0].Nome.Should().Be("Turma A");
        }

        [Fact]
        public async Task GetTurmasByCursoId_DeveRetornarTurmasDoCurso()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma1 = new Turma("Turma A", curso.Id, "ATIVO");
            var turma2 = new Turma("Turma B", curso.Id, "ATIVO");
            _context.Turmas.AddRange(turma1, turma2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTurmasByCursoId(curso.Id);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Nome == "Turma A");
            result.Should().Contain(t => t.Nome == "Turma B");
        }

        [Fact]
        public async Task GetTurmasByCursoId_DeveLancarExcecao_QuandoCursoNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTurmasByCursoId(999));
        }

        [Fact]
        public async Task GetTurmaByAlunoId_DeveRetornarTurmaDoAluno()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "ATIVO");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var matricula = new Matricula { Nome = "Aluno Teste", Email = "aluno@test.com", Cpf = "12345678900", CursoId = curso.Id, Status = StatusMatricula.APROVADO };
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno 
            { 
                Nome = "Aluno Teste", 
                Email = "aluno@test.com", 
                Cpf = "12345678900", 
                Senha = "123", 
                MatriculaId = matricula.Id, 
                TurmaId = turma.Id,
                Status = StatusUsuario.ATIVO 
            };
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTurmaByAlunoId(aluno.Id);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Turma A");
        }

        [Fact]
        public async Task GetTurmaByAlunoId_DeveLancarExcecao_QuandoAlunoNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTurmaByAlunoId(999));
        }

        [Fact]
        public async Task GetQntdTurmasAtivas_DeveRetornarQuantidadeCorreta()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma1 = new Turma("Turma A", curso.Id, "ATIVO");
            var turma2 = new Turma("Turma B", curso.Id, "ATIVO");
            var turma3 = new Turma("Turma C", curso.Id, "ATIVO");
            _context.Turmas.AddRange(turma1, turma2, turma3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetQntdTurmasAtivas();

            // Assert
            result.Should().Be(3);
        }

        [Fact]
        public async Task GetAllTurmas_DeveRetornarTodasTurmas()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma1 = new Turma("Turma A", curso.Id, "ATIVO");
            var turma2 = new Turma("Turma B", curso.Id, "INATIVO");
            _context.Turmas.AddRange(turma1, turma2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllTurmas();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task AtualizarTurma_DeveAtualizarTurmaComSucesso()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma Original", curso.Id, "ATIVO");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var dto = new TurmaDTORequest
            {
                Id = turma.Id,
                Nome = "Turma Atualizada",
                CursoId = curso.Id,
                Status = "INATIVO"
            };

            // Act
            await _service.AtualizarTurma(dto);

            // Assert
            var turmaAtualizada = await _context.Turmas.FindAsync(turma.Id);
            turmaAtualizada.Nome.Should().Be("Turma Atualizada");
            turmaAtualizada.Status.Should().Be("INATIVO");
        }

        [Fact]
        public async Task AtualizarTurma_DeveLancarExcecao_QuandoTurmaNaoExiste()
        {
            // Arrange
            var dto = new TurmaDTORequest
            {
                Id = 999,
                Nome = "Turma Inexistente",
                CursoId = 1,
                Status = "ATIVO"
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.AtualizarTurma(dto));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
