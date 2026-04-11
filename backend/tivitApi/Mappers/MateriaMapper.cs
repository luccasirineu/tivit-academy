using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Materia e MateriaDTO
    /// </summary>
    public static class MateriaMapper
    {
        public static Materia ToEntity(this MateriaDTO dto)
        {
            return new Materia(
                dto.Nome,
                dto.Descricao,
                dto.CursoId
            );
        }
    }
}
