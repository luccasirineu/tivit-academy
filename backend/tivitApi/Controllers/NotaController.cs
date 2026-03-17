using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace tivitApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotaController : ControllerBase
    {
        private readonly INotaService _notaService;
        private readonly ILogger<NotaController> _logger;

        public NotaController(INotaService notaService, ILogger<NotaController> logger)
        {
            _notaService = notaService;
            _logger = logger;
        }

        [Authorize(Roles = "professor")]
        [HttpPost("adicionarNota")]
        [ProducesResponseType(typeof(NotaDTOResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AdicionarNota([FromBody] NotaDTORequest dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { erro = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var notaCriada = await _notaService.AdicionarNotaAsync(dto);
                return CreatedAtAction(
                    nameof(GetAllNotasByAlunoId),
                    new { alunoId = notaCriada.AlunoId },
                    notaCriada
                );
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Dados inválidos ao adicionar nota.");
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar nota para AlunoId={AlunoId} MateriaId={MateriaId}", dto?.AlunoId, dto?.MateriaId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "aluno")]
        [HttpGet("aluno/{alunoId}/getDesempenho")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDesempenhoByAlunoId(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { erro = "AlunoId inválido." });

            try
            {
                var desempenho = await _notaService.GetDesempenhoByAlunoId(alunoId);
                return Ok(desempenho);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisição inválida ao obter desempenho do aluno {AlunoId}", alunoId);
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter desempenho do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        // professor OR aluno
        [Authorize(Roles = "professor,aluno")]
        [HttpGet("aluno/{alunoId}/getAllNotas")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<NotaDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotasByAlunoId(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { erro = "AlunoId inválido." });

            try
            {
                var notas = await _notaService.GetAllNotasByAlunoId(alunoId);
                return Ok(notas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar notas do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        // professor OR aluno
        [Authorize(Roles = "professor,aluno")]
        [HttpGet("aluno/{matriculaId}/getAllNotasByMatriculaId")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<NotaDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotasByMatriculaId(int matriculaId, CancellationToken cancellationToken)
        {
            if (matriculaId <= 0)
                return BadRequest(new { erro = "MatriculaId inválido." });

            try
            {
                var notas = await _notaService.GetAllNotasByMatriculaId(matriculaId);
                return Ok(notas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar notas pela matrícula {MatriculaId}", matriculaId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor")]
        [HttpGet("aluno/getAllNotasByNome")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<NotaDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotasByNome([FromQuery] string nome, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("Nome do aluno é obrigatório.");

            try
            {
                var notas = await _notaService.GetAllNotasByNomeAluno(nome);
                if (notas == null || !notas.Any())
                    return NotFound(new { mensagem = "Nenhuma nota encontrada para o nome informado." });

                return Ok(notas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar notas por nome '{Nome}'", nome);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        [Authorize(Roles = "professor,aluno")]
        [HttpGet("aluno/{alunoId}/exportarRelatorio")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportarRelatorio(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { erro = "AlunoId inválido." });

            // Se for aluno, só permite exportar o próprio relatório
            if (User.IsInRole("aluno"))
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id")?.Value;
                if (!int.TryParse(claim, out var userId) || userId != alunoId)
                    return Forbid();
            }

            try
            {
                var pdfBytes = await _notaService.GerarRelatorioNotasPdfAsync(alunoId);
                if (pdfBytes == null || pdfBytes.Length == 0)
                    return NotFound(new { mensagem = "Relatório não encontrado." });

                return File(pdfBytes, "application/pdf", $"relatorio-aluno-{alunoId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }
    }
}