using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotaController : ControllerBase
    {
        private readonly INotaService _notaService;

        public NotaController(INotaService notaService)
        {
            _notaService = notaService;
        }

        
        [HttpPost("adicionarNota")]
        public async Task<IActionResult> AdicionarNota([FromBody] NotaDTO dto)
        {

            try
            {
                var notaCriada = await _notaService.AdicionarNotaAsync(dto);

                return Created(
                    $"api/Nota/{notaCriada.AlunoId}/{notaCriada.MateriaId}",
                    notaCriada
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }

        [HttpGet("aluno/{alunoId}")]
        public async Task<IActionResult> BuscarNotasPorAluno(int alunoId)
        {
            try
            {
                var notas = await _notaService.BuscarNotasPorAlunoAsync(alunoId);
                return Ok(notas);
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
