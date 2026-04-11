using tivitApi.DTOs;
using tivitApi.Models;

namespace tivitApi.Mappers
{
    /// <summary>
    /// Mapper para conversões entre Conteudo e ConteudoDTO
    /// </summary>
    public static class ConteudoMapper
    {
        public static ConteudoDTO ToDTO(this Conteudo conteudo)
        {
            return new ConteudoDTO(
                conteudo.Id,
                conteudo.Titulo,
                conteudo.Tipo,
                conteudo.CaminhoOuUrl,
                conteudo.DataPublicacao,
                conteudo.MateriaId,
                conteudo.ProfessorId,
                conteudo.TurmaId
            );
        }
    }
}
