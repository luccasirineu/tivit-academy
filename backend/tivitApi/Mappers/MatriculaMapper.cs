using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Matricula e MatriculaDTO
    /// </summary>
    public static class MatriculaMapper
    {
        public static Matricula ToEntity(this MatriculaDTO dto)
        {
            return new Matricula(
                dto.Nome,
                dto.Email,
                dto.Cpf,
                dto.CursoId
            );
        }

        public static MatriculaDTO ToDTO(this Matricula matricula)
        {
            return new MatriculaDTO(
                matricula.Id,
                matricula.Nome,
                matricula.Email,
                matricula.Cpf,
                matricula.Status.ToString(),
                matricula.CursoId
            );
        }
    }
}
