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

        /// <summary>
        /// Adiciona uma nova nota para um aluno.
        /// </summary>
        [Authorize(Roles = "professor")]
        [HttpPost("adicionarNota")]
        [ProducesResponseType(typeof(NotaDTOResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AdicionarNota([FromBody] NotaDTORequest dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { erro = "Payload inv�lido." });

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
                _logger.LogWarning(ex, "Dados inv�lidos ao adicionar nota.");
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar nota para AlunoId={AlunoId} MateriaId={MateriaId}", dto?.AlunoId, dto?.MateriaId);
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }

        /// <summary>
        /// Obtém o desempenho (média, total, etc) de um aluno.
        /// </summary>
        [Authorize(Roles = "aluno")]
        [HttpGet("aluno/{alunoId}/getDesempenho")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDesempenhoByAlunoId(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { erro = "AlunoId inv�lido." });

            try
            {
                var desempenho = await _notaService.GetDesempenhoByAlunoId(alunoId);
                return Ok(desempenho);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Requisi��o inv�lida ao obter desempenho do aluno {AlunoId}", alunoId);
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter desempenho do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }

        // professor OR aluno
        /// <summary>
        /// Obtém todas as notas de um aluno específico.
        /// </summary>
        [Authorize(Roles = "professor,aluno")]
        [HttpGet("aluno/{alunoId}/getAllNotas")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<NotaDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotasByAlunoId(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { erro = "AlunoId inv�lido." });

            try
            {
                var notas = await _notaService.GetAllNotasByAlunoId(alunoId);
                return Ok(notas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar notas do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }

        // professor OR aluno
        /// <summary>
        /// Obtém todas as notas vinculadas a uma matrícula específica.
        /// </summary>
        [Authorize(Roles = "professor,aluno")]
        [HttpGet("aluno/matricula/{matriculaId}/getAllNotasByMatriculaId")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<NotaDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotasByMatriculaId(int matriculaId, CancellationToken cancellationToken)
        {
            if (matriculaId <= 0)
                return BadRequest(new { erro = "MatriculaId inv�lido." });

            try
            {
                var notas = await _notaService.GetAllNotasByMatriculaId(matriculaId);
                return Ok(notas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar notas pela matr�cula {MatriculaId}", matriculaId);
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }

        /// <summary>
        /// Busca notas pesquisando pelo nome do aluno.
        /// </summary>
        [Authorize(Roles = "professor")]
        [HttpGet("aluno/getAllNotasByNome")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<NotaDTOResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllNotasByNome([FromQuery] string nome, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("Nome do aluno � obrigat�rio.");

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
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }

        /// <summary>
        /// Gera e exporta um relatório em PDF com as notas do aluno.
        /// </summary>
        [Authorize(Roles = "professor,aluno")]
        [HttpGet("aluno/{alunoId}/exportarRelatorio")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportarRelatorio(int alunoId, CancellationToken cancellationToken)
        {
            if (alunoId <= 0)
                return BadRequest(new { erro = "AlunoId inv�lido." });

            // Se for aluno, s� permite exportar o pr�prio relat�rio
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
                    return NotFound(new { mensagem = "Relat�rio n�o encontrado." });

                return File(pdfBytes, "application/pdf", $"relatorio-aluno-{alunoId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relat�rio do aluno {AlunoId}", alunoId);
                return Problem(detail: "Erro interno ao processar a requisi��o.", statusCode: 500);
            }
        }
    }
}