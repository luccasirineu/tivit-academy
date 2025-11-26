using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatriculaController : ControllerBase
    {
        //injeção de dependência 
        private readonly IMatriculaService _matriculaService;

        public MatriculaController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarMatricula([FromBody] MatriculaDTO dto)
        {
            Matricula matricula = _matriculaService.ConvertMatriculaDtoToMatricula(dto);

            var created = await _matriculaService.CriarMatriculaAsync(matricula);

            return Ok(new { matriculaId = created.Id });
        }

        [HttpPost("{matriculaId}/pagamento")]
        public async Task<IActionResult> EnviarComprovantePagamento(int matriculaId, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var resultado = await _matriculaService.EnviarComprovantePagamentoAsync(matriculaId, arquivo);

            return Ok(resultado);
        }

        [HttpPost("{matriculaId}/documentos")]
        public async Task<IActionResult> EnviarComprovantePagamento(int matriculaId, IFormFile documentoHistorico, IFormFile documentoCpf)
        {
            if ((documentoHistorico == null || documentoHistorico.Length == 0) || (documentoCpf == null || documentoCpf.Length == 0))
                return BadRequest("Nenhum arquivo enviado.");

            var resultado = await _matriculaService.EnviarDocumentosAsync(matriculaId, documentoHistorico, documentoCpf);

            return Ok(resultado);
        }
    }
}
