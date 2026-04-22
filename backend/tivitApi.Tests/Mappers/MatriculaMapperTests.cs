using FluentAssertions;
using tivitApi.DTOs;
using tivitApi.Enums;
using tivitApi.Mappers;
using tivitApi.Models;

namespace tivitApi.Tests.Mappers
{
    public class MatriculaMapperTests
    {
        [Fact]
        public void MatriculaMapper_ToEntity_DeveConverterCorretamente()
        {
            // Arrange
            var dto = new MatriculaDTO
            {
                Nome = "João Silva",
                Email = "joao.silva@email.com",
                Cpf = "12345678900",
                CursoId = 5
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Nome.Should().Be("João Silva");
            entity.Email.Should().Be("joao.silva@email.com");
            entity.Cpf.Should().Be("12345678900");
            entity.CursoId.Should().Be(5);
            entity.Status.Should().Be(StatusMatricula.AGUARDANDO_PAGAMENTO);
        }

        [Fact]
        public void MatriculaMapper_ToEntity_DeveDefinirStatusInicialComoAguardandoPagamento()
        {
            // Arrange
            var dto = new MatriculaDTO
            {
                Nome = "Maria Santos",
                Email = "maria@email.com",
                Cpf = "98765432100",
                CursoId = 10
            };

            // Act
            var entity = dto.ToEntity();

            // Assert
            entity.Status.Should().Be(StatusMatricula.AGUARDANDO_PAGAMENTO);
        }

        [Fact]
        public void MatriculaMapper_ToDTO_DeveConverterCorretamente()
        {
            // Arrange
            var matricula = new Matricula("Pedro Oliveira", "pedro@email.com", "11122233344", StatusMatricula.APROVADO, 3)
            {
                Id = 1
            };

            // Act
            var dto = matricula.ToDTO();

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(1);
            dto.Nome.Should().Be("Pedro Oliveira");
            dto.Email.Should().Be("pedro@email.com");
            dto.Cpf.Should().Be("11122233344");
            dto.Status.Should().Be("APROVADO");
            dto.CursoId.Should().Be(3);
        }

        [Fact]
        public void MatriculaMapper_ToDTO_DeveConverterStatusParaString()
        {
            // Arrange
            var matricula = new Matricula("Ana Costa", "ana@email.com", "55566677788", StatusMatricula.AGUARDANDO_APROVACAO, 7)
            {
                Id = 2
            };

            // Act
            var dto = matricula.ToDTO();

            // Assert
            dto.Status.Should().Be("AGUARDANDO_APROVACAO");
            dto.Status.Should().BeOfType<string>();
        }

        [Fact]
        public void MatriculaMapper_ToDTO_DevePreservarTodosOsCampos()
        {
            // Arrange
            var matricula = new Matricula("Carlos Ferreira", "carlos@email.com", "99988877766", StatusMatricula.RECUSADO, 15)
            {
                Id = 10
            };

            // Act
            var dto = matricula.ToDTO();

            // Assert
            dto.Id.Should().Be(matricula.Id);
            dto.Nome.Should().Be(matricula.Nome);
            dto.Email.Should().Be(matricula.Email);
            dto.Cpf.Should().Be(matricula.Cpf);
            dto.Status.Should().Be(matricula.Status.ToString());
            dto.CursoId.Should().Be(matricula.CursoId);
        }

        [Fact]
        public void MatriculaMapper_ConversaoIdaEVolta_DeveManterDados()
        {
            // Arrange
            var dtoOriginal = new MatriculaDTO
            {
                Nome = "Teste Completo",
                Email = "teste@email.com",
                Cpf = "12312312312",
                CursoId = 20
            };

            // Act
            var entity = dtoOriginal.ToEntity();
            entity.Id = 5;
            entity.Status = StatusMatricula.APROVADO;
            var dtoFinal = entity.ToDTO();

            // Assert
            dtoFinal.Nome.Should().Be(dtoOriginal.Nome);
            dtoFinal.Email.Should().Be(dtoOriginal.Email);
            dtoFinal.Cpf.Should().Be(dtoOriginal.Cpf);
            dtoFinal.CursoId.Should().Be(dtoOriginal.CursoId);
        }

        [Theory]
        [InlineData(StatusMatricula.AGUARDANDO_PAGAMENTO, "AGUARDANDO_PAGAMENTO")]
        [InlineData(StatusMatricula.AGUARDANDO_DOCUMENTOS, "AGUARDANDO_DOCUMENTOS")]
        [InlineData(StatusMatricula.AGUARDANDO_APROVACAO, "AGUARDANDO_APROVACAO")]
        [InlineData(StatusMatricula.APROVADO, "APROVADO")]
        [InlineData(StatusMatricula.RECUSADO, "RECUSADO")]
        public void MatriculaMapper_ToDTO_DeveConverterTodosOsStatus(StatusMatricula status, string statusEsperado)
        {
            // Arrange
            var matricula = new Matricula("Teste", "teste@email.com", "12345678900", status, 1)
            {
                Id = 1
            };

            // Act
            var dto = matricula.ToDTO();

            // Assert
            dto.Status.Should().Be(statusEsperado);
        }
    }
}
