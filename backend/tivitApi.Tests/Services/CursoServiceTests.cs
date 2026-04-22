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
    public class CursoServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly CursoService _service;

        public CursoServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _service = new CursoService(_context);
        }

        [Fact]
        public async Task GetAllCursosAsync_DeveRetornarTodosCursos()
        {
            // Arrange
            var curso1 = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            var curso2 = new Curso("Medicina", "Curso de Medicina", 2, StatusCurso.ATIVO);
            _context.Cursos.AddRange(curso1, curso2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllCursosAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Nome == "Engenharia");
            result.Should().Contain(c => c.Nome == "Medicina");
        }

        [Fact]
        public async Task GetCursoById_DeveRetornarCursoCorreto()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetCursoById(curso.Id);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Engenharia");
            result.Descricao.Should().Be("Curso de Engenharia");
        }

        [Fact]
        public async Task GetCursoById_DeveLancarExcecao_QuandoCursoNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCursoById(999));
        }

        [Fact]
        public async Task GetQntdCursosProf_DeveRetornarQuantidadeCorreta()
        {
            // Arrange
            var curso1 = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            var curso2 = new Curso("Matemática", "Curso de Matemática", 1, StatusCurso.ATIVO);
            var curso3 = new Curso("Física", "Curso de Física", 2, StatusCurso.ATIVO);
            _context.Cursos.AddRange(curso1, curso2, curso3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetQntdCursosProf(1);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task GetAllCursosProfAsync_DeveRetornarCursosDoProfessor()
        {
            // Arrange
            var curso1 = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            var curso2 = new Curso("Matemática", "Curso de Matemática", 1, StatusCurso.ATIVO);
            var curso3 = new Curso("Física", "Curso de Física", 2, StatusCurso.ATIVO);
            _context.Cursos.AddRange(curso1, curso2, curso3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllCursosProfAsync(1);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Nome == "Engenharia");
            result.Should().Contain(c => c.Nome == "Matemática");
        }

        [Fact]
        public async Task GetQntdAlunosByCursoId_DeveRetornarQuantidadeCorreta()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula1 = new Matricula { Nome = "Aluno 1", Email = "aluno1@test.com", Cpf = "11111111111", CursoId = curso.Id, Status = StatusMatricula.APROVADO };
            var matricula2 = new Matricula { Nome = "Aluno 2", Email = "aluno2@test.com", Cpf = "22222222222", CursoId = curso.Id, Status = StatusMatricula.APROVADO };
            var matricula3 = new Matricula { Nome = "Aluno 3", Email = "aluno3@test.com", Cpf = "33333333333", CursoId = curso.Id, Status = StatusMatricula.AGUARDANDO_APROVACAO };
            _context.Matriculas.AddRange(matricula1, matricula2, matricula3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetQntdAlunosByCursoId(curso.Id);

            // Assert
            result.Should().Be(2); // Apenas aprovados
        }

        [Fact]
        public async Task CriarCurso_DeveCriarCursoComSucesso()
        {
            // Arrange
            var dto = new CursoDTORequest
            {
                Nome = "Novo Curso",
                Descricao = "Descrição do novo curso",
                ProfResponsavel = 1
            };

            // Act
            await _service.CriarCurso(dto);

            // Assert
            var cursos = await _service.GetAllCursosAsync();
            cursos.Should().HaveCount(1);
            cursos[0].Nome.Should().Be("Novo Curso");
        }

        [Fact]
        public async Task AtualizarCurso_DeveAtualizarCursoComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Original", "Descrição Original", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var dto = new CursoDTORequest
            {
                Id = curso.Id,
                Nome = "Curso Atualizado",
                Descricao = "Descrição Atualizada",
                ProfResponsavel = 2
            };

            // Act
            await _service.AtualizarCurso(dto);

            // Assert
            var cursoAtualizado = await _service.GetCursoById(curso.Id);
            cursoAtualizado.Nome.Should().Be("Curso Atualizado");
            cursoAtualizado.Descricao.Should().Be("Descrição Atualizada");
        }

        [Fact]
        public async Task AtualizarCurso_DeveLancarExcecao_QuandoCursoNaoExiste()
        {
            // Arrange
            var dto = new CursoDTORequest
            {
                Id = 999,
                Nome = "Curso Inexistente",
                Descricao = "Descrição",
                ProfResponsavel = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.AtualizarCurso(dto));
        }

        [Fact]
        public async Task DesativarCurso_DeveDesativarCursoComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Ativo", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            // Act
            await _service.DesativarCurso(curso.Id);

            // Assert
            var cursoDesativado = await _context.Cursos.FindAsync(curso.Id);
            cursoDesativado.Status.Should().Be(StatusCurso.DESATIVADO);
        }

        [Fact]
        public async Task AtivarCurso_DeveAtivarCursoComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Desativado", "Descrição", 1, StatusCurso.DESATIVADO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            // Act
            await _service.AtivarCurso(curso.Id);

            // Assert
            var cursoAtivado = await _context.Cursos.FindAsync(curso.Id);
            cursoAtivado.Status.Should().Be(StatusCurso.ATIVO);
        }

        [Fact]
        public async Task GetAllCursosAtivos_DeveRetornarApenasCursosAtivos()
        {
            // Arrange
            var curso1 = new Curso("Curso Ativo 1", "Descrição", 1, StatusCurso.ATIVO);
            var curso2 = new Curso("Curso Ativo 2", "Descrição", 1, StatusCurso.ATIVO);
            var curso3 = new Curso("Curso Desativado", "Descrição", 1, StatusCurso.DESATIVADO);
            _context.Cursos.AddRange(curso1, curso2, curso3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllCursosAtivos();

            // Assert
            result.Should().HaveCount(2);
            result.Should().NotContain(c => c.Nome == "Curso Desativado");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
