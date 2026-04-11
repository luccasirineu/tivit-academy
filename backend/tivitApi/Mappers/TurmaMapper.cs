using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Turma e TurmaDTO
    /// </summary>
    public static class TurmaMapper
    {
        public static TurmaDTOResponse ToDTO(this Turma turma)
        {
            return new TurmaDTOResponse(
                turma.Id,
                turma.Nome,
                turma.CursoId,
                turma.Status
            );
        }

        public static Turma ToEntity(this TurmaDTORequest dto)
        {
            return new Turma(
                dto.Nome,
                dto.CursoId,
                dto.Status
            );
        }
    }
}
