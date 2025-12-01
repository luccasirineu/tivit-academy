using tivitApi.Models;
using tivitApi.DTOs;

namespace tivitApi.Services
{
    public interface IMatriculaService
    {
        Task<Matricula> CriarMatriculaAsync(MatriculaDTO matriculaDTO);
        Task<ComprovantePagamentoDTO> EnviarComprovantePagamentoAsync(int matriculaId, IFormFile arquivo);
        Task<DocumentosDTO> EnviarDocumentosAsync(int matriculaId, IFormFile documentoHistorico, IFormFile documentoCpf);
        Task<List<MatriculaDTO>> GetAllMatriculas();
    }
}