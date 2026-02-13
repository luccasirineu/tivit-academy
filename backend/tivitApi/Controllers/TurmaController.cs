using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurmaController : ControllerBase
    {
        private readonly ITurmaService _turmaService;

        public TurmaController(ITurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        [HttpPost("criarTurma")]
        public async Task<IActionResult> CriarTurma([FromBody] TurmaDTORequest turmaDTO)
        {
            try
            {
                await _turmaService.CriarTurma(turmaDTO);

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

        [HttpGet("getTurmasByCursoId/{cursoId}")]
        public async Task<IActionResult> GetTurmasByCursoId(int cursoId)
        {
            var turmas = await _turmaService.GetTurmasByCursoId(cursoId);

            return Ok(turmas);
        }

        [HttpGet("getTurmaByAlunoId/{alunoId}")]
        public async Task<IActionResult> GetTurmaByAlunoId(int alunoId)
        {
            var turmaId = await _turmaService.GetTurmaByAlunoId(alunoId);

            return Ok(turmaId);
        }

        [HttpGet("getQntdTurmasAtivas")]
        public async Task<IActionResult> GetQntdTurmasAtivas()
        {

            try
            {
                var qntdTurmasAtivas = await _turmaService.GetQntdTurmasAtivas();
                return Ok(qntdTurmasAtivas);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getAllTurmas")]
        public async Task<IActionResult> GetAllTurmas()
        {
            List<TurmaDTOResponse> turmas = await _turmaService.GetAllTurmas();

            return Ok(turmas);
        }

        [HttpPut("atualizarTurma")]
        public async Task<IActionResult> AtualizarTurma([FromBody] TurmaDTORequest dto)
        {

            try
            {
                await _turmaService.AtualizarTurma(dto);

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
