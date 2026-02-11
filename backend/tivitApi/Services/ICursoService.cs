using tivitApi.Models;
using tivitApi.DTOs;

namespace tivitApi.Services
{
    public interface ICursoService
    {

        Task<List<CursoDTO>> GetAllCursosAsync();
        Task<CursoDTO> GetCursoById(int cursoId);
        Task<int> GetQntdCursosProf(int professorId);
        Task<List<CursoDTO>> GetAllCursosProfAsync(int professorId);
        Task<int> GetQntdAlunosByCursoId(int cursoId);
        Task CriarCurso(CursoDTORequest dto);
        Task AtualizarCurso(CursoDTO dto);
        Task DesativarCurso(int cursoId);
        Task AtivarCurso(int cursoId);


    }
}