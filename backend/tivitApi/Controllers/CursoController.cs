using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursoController : ControllerBase
    {
        //injeção de dependência 
        private readonly ICursoService _cursoService;

        public CursoController(ICursoService cursoService)
        {
            _cursoService = cursoService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCursos()
        {
            List<CursoDTO> cursosDTOs = await _cursoService.GetAllCursosAsync();

            return Ok(cursosDTOs);
        }

        [HttpGet("{cursoId}")]
        public async Task<IActionResult> GetCursoId(int cursoId)
        {
            CursoDTO cursoDTO = await _cursoService.GetCursoById(cursoId);

            return Ok(cursoDTO);
        }

        [HttpGet("getQntdCursosProf/{professorId}")]
        public async Task<IActionResult> GetNextWeekEvents(int professorId)
        {
            var qntdEventos = await _cursoService.GetQntdCursosProf(professorId);


            return Ok(qntdEventos);
        }

        [HttpGet("getAllCursosProf/{professorId}")]
        public async Task<IActionResult> GetAllCursosProfAsync(int professorId)
        {
            var cursosProfessor = await _cursoService.GetAllCursosProfAsync(professorId);


            return Ok(cursosProfessor);
        }

    }
}
