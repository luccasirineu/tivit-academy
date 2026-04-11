using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Nota e NotaDTO
    /// </summary>
    public static class NotaMapper
    {
        public static Nota ToEntity(this NotaDTORequest dto, decimal media, string status, int qntdFaltas)
        {
            return new Nota(
                dto.AlunoId,
                dto.MateriaId,
                dto.Nota1,
                dto.Nota2,
                media,
                qntdFaltas,
                status
            );
        }
    }
}
