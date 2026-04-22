using FluentAssertions;
using tivitApi.Data;
using tivitApi.Enums;
using tivitApi.Exceptions;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _service = new UserService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetUserByCpf_DeveRetornarAluno_QuandoCpfExiste()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João Silva", "joao@email.com", "12345678900", "senha123", matricula.Id);
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUserByCpf("12345678900");

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("João Silva");
            result.Tipo.Should().Be("Aluno");
        }

        [Fact]
        public async Task GetUserByCpf_DeveRetornarProfessor_QuandoCpfExiste()
        {
            // Arrange
            var professor = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO)
            {
                Senha = "senhaHash"
            };
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUserByCpf("98765432100");

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Maria Santos");
            result.Tipo.Should().Be("Professor");
        }

        [Fact]
        public async Task GetUserByCpf_DeveLancarExcecao_QuandoCpfNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetUserByCpf("00000000000"));
        }

        [Fact]
        public async Task GetUsersByNome_DeveRetornarUsuarios_QuandoNomeExiste()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João Silva", "joao@email.com", "12345678900", "senha123", matricula.Id);
            _context.Alunos.Add(aluno);

            var professor = new Professor("João Santos", "joao.santos@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO)
            {
                Senha = "senhaHash"
            };
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUsersByNome("João");

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetUsersByNome_DeveLancarExcecao_QuandoNomeNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetUsersByNome("Inexistente"));
        }

        [Fact]
        public async Task DesativarUser_DeveDesativarAluno_QuandoTipoAluno()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João Silva", "joao@email.com", "12345678900", "senha123", matricula.Id);
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            await _service.DesativarUser("12345678900", "aluno");

            // Assert
            var alunoAtualizado = _context.Alunos.First();
            alunoAtualizado.Status.Should().Be(StatusUsuario.DESATIVADO);
        }

        [Fact]
        public async Task DesativarUser_DeveDesativarProfessor_QuandoTipoProfessor()
        {
            // Arrange
            var professor = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO)
            {
                Senha = "senhaHash"
            };
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            // Act
            await _service.DesativarUser("98765432100", "professor");

            // Assert
            var professorAtualizado = _context.Professores.First();
            professorAtualizado.Status.Should().Be(StatusUsuario.DESATIVADO);
        }

        [Fact]
        public async Task DesativarUser_DeveLancarExcecao_QuandoCpfInvalido()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.DesativarUser("", "aluno"));
        }

        [Fact]
        public async Task DesativarUser_DeveLancarExcecao_QuandoTipoInvalido()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.DesativarUser("12345678900", "invalido"));
        }

        [Fact]
        public async Task AtivarUser_DeveAtivarAluno_QuandoTipoAluno()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno("João Silva", "joao@email.com", "12345678900", "senha123", matricula.Id);
            aluno.Status = StatusUsuario.DESATIVADO;
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            await _service.AtivarUser("12345678900", "aluno");

            // Assert
            var alunoAtualizado = _context.Alunos.First();
            alunoAtualizado.Status.Should().Be(StatusUsuario.ATIVO);
        }

        [Fact]
        public async Task AtivarUser_DeveAtivarProfessor_QuandoTipoProfessor()
        {
            // Arrange
            var professor = new Professor("Maria Santos", "maria@email.com", "RM123456", "98765432100", StatusUsuario.ATIVO)
            {
                Senha = "senhaHash",
                Status = StatusUsuario.DESATIVADO
            };
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            // Act
            await _service.AtivarUser("98765432100", "professor");

            // Assert
            var professorAtualizado = _context.Professores.First();
            professorAtualizado.Status.Should().Be(StatusUsuario.ATIVO);
        }

        [Fact]
        public async Task AtivarUser_DeveLancarExcecao_QuandoCpfInvalido()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.AtivarUser("", "aluno"));
        }

        [Fact]
        public async Task AtivarUser_DeveLancarExcecao_QuandoTipoInvalido()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.AtivarUser("12345678900", "invalido"));
        }
    }
}
