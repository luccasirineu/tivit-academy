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
    public class AlunoServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ISQSProducer > _queueMock;
        private readonly Mock<ILogger<AlunoService>> _loggerMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly AlunoService _service;

        public AlunoServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _queueMock = new Mock<ISQSProducer >();
            _loggerMock = new Mock<ILogger<AlunoService>>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashedPassword");
            
            _service = new AlunoService(_context, _queueMock.Object, _loggerMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task GetInfoAluno_DeveRetornarInformacoesDoAluno()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula = new Matricula 
            { 
                Nome = "João Silva", 
                Email = "joao@test.com", 
                Cpf = "12345678900", 
                CursoId = curso.Id, 
                Status = StatusMatricula.APROVADO 
            };
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno 
            { 
                Nome = "João Silva", 
                Email = "joao@test.com", 
                Cpf = "12345678900", 
                Senha = "123", 
                MatriculaId = matricula.Id, 
                Status = StatusUsuario.ATIVO,
                TurmaId = 1
            };
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetInfoAluno(aluno.Id);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("João Silva");
            result.Email.Should().Be("joao@test.com");
            result.CursoNome.Should().Be("Engenharia");
        }

        [Fact]
        public async Task GetInfoAluno_DeveLancarExcecao_QuandoAlunoNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetInfoAluno(999));
        }

        [Fact]
        public async Task GetAllAlunosByCurso_DeveRetornarAlunosAprovados()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var matricula1 = new Matricula 
            { 
                Nome = "Aluno 1", 
                Email = "aluno1@test.com", 
                Cpf = "11111111111", 
                CursoId = curso.Id, 
                Status = StatusMatricula.APROVADO 
            };
            var matricula2 = new Matricula 
            { 
                Nome = "Aluno 2", 
                Email = "aluno2@test.com", 
                Cpf = "22222222222", 
                CursoId = curso.Id, 
                Status = StatusMatricula.APROVADO 
            };
            _context.Matriculas.AddRange(matricula1, matricula2);
            await _context.SaveChangesAsync();

            var aluno1 = new Aluno { Nome = "Aluno 1", Email = "aluno1@test.com", Cpf = "11111111111", Senha = "123", MatriculaId = matricula1.Id, Status = StatusUsuario.ATIVO };
            var aluno2 = new Aluno { Nome = "Aluno 2", Email = "aluno2@test.com", Cpf = "22222222222", Senha = "123", MatriculaId = matricula2.Id, Status = StatusUsuario.ATIVO };
            _context.Alunos.AddRange(aluno1, aluno2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAlunosByCurso(curso.Id);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAlunosByTurmaId_DeveRetornarAlunosDaTurma()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "ATIVO");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var matricula = new Matricula 
            { 
                Nome = "Aluno Teste", 
                Email = "aluno@test.com", 
                Cpf = "12345678900", 
                CursoId = curso.Id, 
                Status = StatusMatricula.APROVADO 
            };
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
            var result = await _service.GetAllAlunosByTurmaId(turma.Id);

            // Assert
            result.Should().HaveCount(1);
            result[0].Nome.Should().Be("Aluno Teste");
        }

        [Fact]
        public async Task GetAllAlunosByTurmaId_DeveLancarExcecao_QuandoTurmaNaoExiste()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllAlunosByTurmaId(999));
        }

        [Fact]
        public async Task GetAlunoByMatriculaId_DeveRetornarAluno()
        {
            // Arrange
            var matricula = new Matricula 
            { 
                Nome = "Teste", 
                Email = "teste@test.com", 
                Cpf = "12345678900", 
                CursoId = 1, 
                Status = StatusMatricula.APROVADO 
            };
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno 
            { 
                Nome = "Teste", 
                Email = "teste@test.com", 
                Cpf = "12345678900", 
                Senha = "123", 
                MatriculaId = matricula.Id, 
                Status = StatusUsuario.ATIVO 
            };
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAlunoByMatriculaId(matricula.Id);

            // Assert
            result.Should().NotBeNull();
            result.MatriculaId.Should().Be(matricula.Id);
        }

        [Fact]
        public async Task GetQntdAlunosAtivos_DeveRetornarQuantidadeCorreta()
        {
            // Arrange
            var matricula1 = new Matricula { Nome = "Aluno 1", Email = "aluno1@test.com", Cpf = "11111111111", CursoId = 1, Status = StatusMatricula.APROVADO };
            var matricula2 = new Matricula { Nome = "Aluno 2", Email = "aluno2@test.com", Cpf = "22222222222", CursoId = 1, Status = StatusMatricula.APROVADO };
            _context.Matriculas.AddRange(matricula1, matricula2);
            await _context.SaveChangesAsync();

            var aluno1 = new Aluno { Nome = "Aluno 1", Email = "aluno1@test.com", Cpf = "11111111111", Senha = "123", MatriculaId = matricula1.Id, Status = StatusUsuario.ATIVO };
            var aluno2 = new Aluno { Nome = "Aluno 2", Email = "aluno2@test.com", Cpf = "22222222222", Senha = "123", MatriculaId = matricula2.Id, Status = StatusUsuario.ATIVO };
            _context.Alunos.AddRange(aluno1, aluno2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetQntdAlunosAtivos();

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public async Task UpdateTurmaAluno_DeveAtualizarTurmaDoAluno()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "ATIVO");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var matricula = new Matricula { Nome = "Teste", Email = "teste@test.com", Cpf = "12345678900", CursoId = curso.Id, Status = StatusMatricula.APROVADO };
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var aluno = new Aluno { Nome = "Teste", Email = "teste@test.com", Cpf = "12345678900", Senha = "123", MatriculaId = matricula.Id, Status = StatusUsuario.ATIVO };
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Act
            await _service.UpdateTurmaAluno(aluno.Id, turma.Id);

            // Assert
            var alunoAtualizado = await _context.Alunos.FindAsync(aluno.Id);
            alunoAtualizado.TurmaId.Should().Be(turma.Id);
        }

        [Fact]
        public async Task UpdateTurmaAluno_DeveLancarExcecao_QuandoAlunoNaoExiste()
        {
            // Arrange
            var curso = new Curso("Engenharia", "Curso de Engenharia", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "ATIVO");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateTurmaAluno(999, turma.Id));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
