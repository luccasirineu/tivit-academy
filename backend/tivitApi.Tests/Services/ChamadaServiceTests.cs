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
    public class ChamadaServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ChamadaService _service;

        public ChamadaServiceTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _service = new ChamadaService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task RealizarChamada_DeveCriarChamadasComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var materia = new Materia("Matemática", "Descrição", curso.Id);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var dtos = new List<ChamadaDTO>
            {
                new ChamadaDTO
                {
                    MatriculaId = matricula.Id,
                    MateriaId = materia.Id,
                    TurmaId = turma.Id,
                    Faltou = false
                }
            };

            // Act
            await _service.RealizarChamada(dtos);

            // Assert
            var chamadas = _context.Chamadas.ToList();
            chamadas.Should().HaveCount(1);
            chamadas[0].MatriculaId.Should().Be(matricula.Id);
            chamadas[0].Faltou.Should().BeFalse();
        }

        [Fact]
        public async Task RealizarChamada_DeveLancarExcecao_QuandoChamadaJaExiste()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var materia = new Materia("Matemática", "Descrição", curso.Id);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var chamadaExistente = new Chamada(matricula.Id, materia.Id, false, DateTime.UtcNow, turma.Id);
            _context.Chamadas.Add(chamadaExistente);
            await _context.SaveChangesAsync();

            var dtos = new List<ChamadaDTO>
            {
                new ChamadaDTO
                {
                    MatriculaId = matricula.Id,
                    MateriaId = materia.Id,
                    TurmaId = turma.Id,
                    Faltou = false
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _service.RealizarChamada(dtos));
        }

        [Fact]
        public async Task RealizarChamada_NaoDeveFazerNada_QuandoListaVazia()
        {
            // Arrange
            var dtos = new List<ChamadaDTO>();

            // Act
            await _service.RealizarChamada(dtos);

            // Assert
            var chamadas = _context.Chamadas.ToList();
            chamadas.Should().BeEmpty();
        }

        [Fact]
        public async Task AtualizarChamada_DeveAtualizarChamadasComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso Teste", "Descrição", 1, StatusCurso.ATIVO);
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            var turma = new Turma("Turma A", curso.Id, "Ativa");
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            var materia = new Materia("Matemática", "Descrição", curso.Id);
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            var matricula = new Matricula("João Silva", "joao@email.com", "12345678900", curso.Id);
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            var chamadaExistente = new Chamada(matricula.Id, materia.Id, false, DateTime.UtcNow, turma.Id);
            _context.Chamadas.Add(chamadaExistente);
            await _context.SaveChangesAsync();

            var dtos = new List<ChamadaDTO>
            {
                new ChamadaDTO
                {
                    MatriculaId = matricula.Id,
                    MateriaId = materia.Id,
                    TurmaId = turma.Id,
                    Faltou = true
                }
            };

            // Act
            await _service.AtualizarChamada(dtos);

            // Assert
            var chamadaAtualizada = _context.Chamadas.First();
            chamadaAtualizada.Faltou.Should().BeTrue();
        }

        [Fact]
        public async Task AtualizarChamada_DeveLancarExcecao_QuandoChamadaNaoExiste()
        {
            // Arrange
            var dtos = new List<ChamadaDTO>
            {
                new ChamadaDTO
                {
                    MatriculaId = 1,
                    MateriaId = 1,
                    TurmaId = 1,
                    Faltou = true
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _service.AtualizarChamada(dtos));
        }

        [Fact]
        public async Task AtualizarChamada_NaoDeveFazerNada_QuandoListaVazia()
        {
            // Arrange
            var dtos = new List<ChamadaDTO>();

            // Act
            await _service.AtualizarChamada(dtos);

            // Assert - Não deve lançar exceção
            var chamadas = _context.Chamadas.ToList();
            chamadas.Should().BeEmpty();
        }
    }
}
