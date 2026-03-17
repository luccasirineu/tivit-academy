using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MatriculaController : ControllerBase
    {
        private readonly IMatriculaService _matriculaService;
        private readonly ILogger<MatriculaController> _logger;

        public MatriculaController(IMatriculaService matriculaService, ILogger<MatriculaController> logger)
        {
            _matriculaService = matriculaService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CriarMatricula([FromBody] MatriculaDTO dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matriculaCriada = await _matriculaService.CriarMatriculaAsync(dto);
            return CreatedAtAction(nameof(GetMatriculaById), new { id = matriculaCriada.Id }, new { matriculaId = matriculaCriada.Id });
        }

        [Authorize(Roles = "administrador,professor")]
        [HttpGet("getAllMatriculasPendentes")]
        [ProducesResponseType(typeof(MatriculaDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMatriculaById( CancellationToken cancellationToken)
        {
            var m = await _matriculaService.GetAllMatriculasPendentes(); 
            return Ok(m);
        }

        [AllowAnonymous]
        [HttpPost("{matriculaId}/pagamento")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EnviarComprovantePagamento(int matriculaId, IFormFile arquivo)
        {
            if (matriculaId <= 0)
                return BadRequest(new { message = "MatriculaId inválido." });

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest(new { message = "Nenhum arquivo enviado." });


            var resultado = await _matriculaService.EnviarComprovantePagamentoAsync(matriculaId, arquivo);

            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpPost("{matriculaId:int}/documentos")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EnviarDocumentos(int matriculaId, [FromForm] IFormFile documentoHistorico, [FromForm] IFormFile documentoCpf, CancellationToken cancellationToken)
        {
            if (matriculaId <= 0)
                return BadRequest(new { message = "MatriculaId inválido." });

            if (documentoHistorico == null || documentoCpf == null)
                return BadRequest(new { message = "Ambos os documentos são obrigatórios." });

            var resultado = await _matriculaService.EnviarDocumentosAsync(matriculaId, documentoHistorico, documentoCpf);
            return Ok(resultado);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("pendentes")]
        [ProducesResponseType(typeof(List<MatriculaDTO>), 200)]
        public async Task<IActionResult> GetAllMatriculasPendentes(CancellationToken cancellationToken)
        {
            var matriculaDTOs = await _matriculaService.GetAllMatriculasPendentes();
            return Ok(matriculaDTOs);
        }

        [Authorize(Roles = "administrador")]
        [HttpPost("aprovar/{matriculaId:int}")]
        public async Task<IActionResult> AprovarMatricula(int matriculaId)
        {
            await _matriculaService.AprovarMatricula(matriculaId.ToString()); // ideal: padronizar service para int
            return NoContent();
        }

        [Authorize(Roles = "administrador")]
        [HttpPost("recusar/{matriculaId:int}")]
        public async Task<IActionResult> RecusarMatricula(int matriculaId)
        {
            await _matriculaService.RecusarMatricula(matriculaId.ToString());
            return NoContent();
        }

        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getAlunosAtivosProfessor/{professorId:int}")]
        public async Task<IActionResult> GetAlunosAtivosProfessor(int professorId)
        {
            var qntdAlunosAtivos = await _matriculaService.GetTotalAlunosAtivosPorProfessor(professorId);
            return Ok(qntdAlunosAtivos);
        }
    }
}