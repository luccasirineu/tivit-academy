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

    }
}
