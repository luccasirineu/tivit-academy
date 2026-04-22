using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Exceptions;
using tivitApi.Infra.SQS;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Tests.Helpers;

namespace tivitApi.Tests.Services
{
    public class MatriculaServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<MatriculaService>> _loggerMock;
        private readonly Mock<ISQSProducer> _queueMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly MatriculaService _service;

        public MatriculaServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _loggerMock = new Mock<ILogger<MatriculaService>>();
            _queueMock = new Mock<ISQSProducer>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashedPassword");
            
            _service = new MatriculaService(_context, _loggerMock.Object, _queueMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task CriarMatriculaAsync_DeveCriarMatriculaComSucesso()
        {
            // Arrange
            var dto = new MatriculaDTO
            {
                Nome = "João Silva",
                Email = "joao@test.com",
                Cpf = "123.456.789-00",
                CursoId = 1
            };

            // Act
            var result = await _service.CriarMatriculaAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("João Silva");
            result.Cpf.Should().Be("12345678900"); // CPF sem formatação
            result.Status.Should().Be(StatusMatricula.AGUARDANDO_PAGAMENTO);
        }

        [Fact]
        public async Task CriarMatriculaAsync_DeveLancarExcecao_QuandoCpfJaMatriculado()
        {
            // Arrange
            var matriculaExistente = new Matricula
            {
                Nome = "João Silva",
                Email = "joao@test.com",
                Cpf = "12345678900",
                CursoId = 1,
                Status = StatusMatricula.AGUARDANDO_APROVACAO
            };
            _context.Matriculas.Add(matriculaExistente);
            await _context.SaveChangesAsync();

            var dto = new MatriculaDTO
            {
                Nome = "João Silva",
                Email = "joao@test.com",
                Cpf = "123.456.789-00",
                CursoId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _service.CriarMatriculaAsync(dto));
        }

        [Fact]
        public async Task GetAllMatriculasPendentes_DeveRetornarMatriculasAguardandoAprovacao()
        {
            // Arrange
            var matricula1 = new Matricula { Nome = "Aluno 1", Email = "aluno1@test.com", Cpf = "11111111111", CursoId = 1, Status = StatusMatricula.AGUARDANDO_APROVACAO };
            var matricula2 = new Matricula { Nome = "Aluno 2", Email = "aluno2@test.com", Cpf = "22222222222", CursoId = 1, Status = StatusMatricula.AGUARDANDO_APROVACAO };
            var matricula3 = new Matricula { Nome = "Aluno 3", Email = "aluno3@test.com", Cpf = "33333333333", CursoId = 1, Status = StatusMatricula.APROVADO };
            _context.Matriculas.AddRange(matricula1, matricula2, matricula3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllMatriculasPendentes();

            // Assert
            result.Should().HaveCount(2);
            result.Should().NotContain(m => m.Nome == "Aluno 3");
        }

        [Fact]
        public async Task AprovarMatricula_DeveAprovarMatriculaECriarAluno()
        {
            // Arrange
            var matricula = new Matricula
            {
                Nome = "João Silva",
                Email = "joao@test.com",
                Cpf = "12345678900",
                CursoId = 1,
                Status = StatusMatricula.AGUARDANDO_APROVACAO
            };
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            // Act
            await _service.AprovarMatricula(matricula.Id.ToString());

            // Assert
            var matriculaAtualizada = await _context.Matriculas.FindAsync(matricula.Id);
            matriculaAtualizada.Status.Should().Be(StatusMatricula.APROVADO);

            var alunoCriado = _context.Alunos.FirstOrDefault(a => a.MatriculaId == matricula.Id);
            alunoCriado.Should().NotBeNull();
            alunoCriado.Nome.Should().Be("João Silva");
        }

        [Fact]
        public async Task RecusarMatricula_DeveRecusarMatricula()
        {
            // Arrange
            var matricula = new Matricula
            {
                Nome = "João Silva",
                Email = "joao@test.com",
                Cpf = "12345678900",
                CursoId = 1,
                Status = StatusMatricula.AGUARDANDO_APROVACAO
            };
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            // Act
            await _service.RecusarMatricula(matricula.Id.ToString());

            // Assert
            var matriculaAtualizada = await _context.Matriculas.FindAsync(matricula.Id);
            matriculaAtualizada.Status.Should().Be(StatusMatricula.RECUSADO);
        }

        [Fact]
        public async Task GetTotalAlunosAtivosPorProfessor_DeveRetornarQuantidadeCorreta()
        {
            // Arrange
            var curso1 = new Curso("Curso 1", "Descrição", 1, StatusCurso.ATIVO);
            var curso2 = new Curso("Curso 2", "Descrição", 1, StatusCurso.ATIVO);
            var curso3 = new Curso("Curso 3", "Descrição", 2, StatusCurso.ATIVO);
            _context.Cursos.AddRange(curso1, curso2, curso3);
            await _context.SaveChangesAsync();

            var matricula1 = new Matricula { Nome = "Aluno 1", Email = "aluno1@test.com", Cpf = "11111111111", CursoId = curso1.Id, Status = StatusMatricula.APROVADO };
            var matricula2 = new Matricula { Nome = "Aluno 2", Email = "aluno2@test.com", Cpf = "22222222222", CursoId = curso2.Id, Status = StatusMatricula.APROVADO };
            var matricula3 = new Matricula { Nome = "Aluno 3", Email = "aluno3@test.com", Cpf = "33333333333", CursoId = curso3.Id, Status = StatusMatricula.APROVADO };
            _context.Matriculas.AddRange(matricula1, matricula2, matricula3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTotalAlunosAtivosPorProfessor(1);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task GetTotalAlunosAtivosPorProfessor_DeveRetornarZero_QuandoProfessorNaoTemCursos()
        {
            // Act
            var result = await _service.GetTotalAlunosAtivosPorProfessor(999);

            // Assert
            result.Should().Be(0);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
