using tivitApi.Models;
using tivitApi.DTOs;

namespace tivitApi.Services
{
    public interface ICursoService
    {
         CursoDTO ConvertCursoToCursoDTO(Curso curso);

        Task<List<CursoDTO>> GetAllCursosAsync();
        Task<CursoDTO> GetCursoById(int cursoId);
        Task<int> GetQntdCursosProf(int professorId);



    }
}