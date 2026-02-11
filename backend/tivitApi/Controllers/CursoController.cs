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

        [HttpGet("getQntdAlunosByCursoId/{cursoId}")]
        public async Task<IActionResult> GetQntdAlunosByCursoId(int cursoId)
        {
            var qntdAlunos = await _cursoService.GetQntdAlunosByCursoId(cursoId);


            return Ok(qntdAlunos);
        }

        [HttpPost("criarCurso")]
        public async Task<IActionResult> CriarCurso([FromBody] CursoDTORequest dto)
        {
            await _cursoService.CriarCurso(dto);
            return NoContent();

        }

        [HttpPut("atualizarCurso")]
        public async Task<IActionResult> AtualizarCurso([FromBody] CursoDTO dto)
        {

            try
            {
                await _cursoService.AtualizarCurso(dto);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }

        [HttpPut("desativarCurso/{cursoId}")]
        public async Task<IActionResult> DesativarCurso(int cursoId)
        {

            try
            {
                await _cursoService.DesativarCurso(cursoId);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }

        [HttpPut("ativarCurso/{cursoId}")]
        public async Task<IActionResult> AtivarCurso(int cursoId)
        {

            try
            {
                await _cursoService.AtivarCurso(cursoId);

                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }
    }
}
