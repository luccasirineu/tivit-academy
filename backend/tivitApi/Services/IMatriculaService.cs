using tivitApi.Models;
using tivitApi.DTOs;

namespace tivitApi.Services
{
    public interface IMatriculaService
    {
        Task<Matricula> CriarMatriculaAsync(Matricula matricula);
        Matricula ConvertMatriculaDtoToMatricula(MatriculaDTO dto);

    }
}