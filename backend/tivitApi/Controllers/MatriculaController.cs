﻿using System.Collections.Generic;
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

        /// <summary>
        /// Cria uma nova matrícula.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CriarMatricula([FromBody] MatriculaDTO dto, CancellationToken cancellationToken)
        {
            if (dto == null)
                return BadRequest(new { message = "Payload inv�lido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matriculaCriada = await _matriculaService.CriarMatriculaAsync(dto);
            return CreatedAtAction(nameof(GetMatriculaById), new { id = matriculaCriada.Id }, new { matriculaId = matriculaCriada.Id });
        }

        /// <summary>
        /// Retorna todas as matrículas pendentes.
        /// </summary>
        [Authorize(Roles = "administrador,professor")]
        [HttpGet("getAllMatriculasPendentes")]
        [ProducesResponseType(typeof(MatriculaDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMatriculaById( CancellationToken cancellationToken)
        {
            var m = await _matriculaService.GetAllMatriculasPendentes(); 
            return Ok(m);
        }

        /// <summary>
        /// Envia o comprovante de pagamento para uma matrícula específica.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("{matriculaId}/pagamento")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EnviarComprovantePagamento([FromRoute] int matriculaId, IFormFile arquivo)
        {
            if (matriculaId <= 0)
                return BadRequest(new { message = "MatriculaId inv�lido." });

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest(new { message = "Nenhum arquivo enviado." });


            var resultado = await _matriculaService.EnviarComprovantePagamentoAsync(matriculaId, arquivo);

            return Ok(resultado);
        }

        /// <summary>
        /// Envia os documentos necessários para a matrícula (Histórico e CPF).
        /// </summary>
        [AllowAnonymous]
        [HttpPost("{matriculaId:int}/documentos")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EnviarDocumentos([FromRoute] int matriculaId, IFormFile documentoHistorico, IFormFile documentoCpf, CancellationToken cancellationToken)
        {
            if (matriculaId <= 0)
                return BadRequest(new { message = "MatriculaId inv�lido." });

            if (documentoHistorico == null || documentoCpf == null)
                return BadRequest(new { message = "Ambos os documentos s�o obrigat�rios." });

            var resultado = await _matriculaService.EnviarDocumentosAsync(matriculaId, documentoHistorico, documentoCpf);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtém a lista de todas as matrículas pendentes.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpGet("pendentes")]
        [ProducesResponseType(typeof(List<MatriculaDTO>), 200)]
        public async Task<IActionResult> GetAllMatriculasPendentes(CancellationToken cancellationToken)
        {
            var matriculaDTOs = await _matriculaService.GetAllMatriculasPendentes();
            return Ok(matriculaDTOs);
        }

        /// <summary>
        /// Aprova uma matrícula específica.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpPost("aprovar/{matriculaId:int}")]
        public async Task<IActionResult> AprovarMatricula(int matriculaId)
        {
            await _matriculaService.AprovarMatricula(matriculaId.ToString()); // ideal: padronizar service para int
            return NoContent();
        }

        /// <summary>
        /// Recusa uma matrícula específica.
        /// </summary>
        [Authorize(Roles = "administrador")]
        [HttpPost("recusar/{matriculaId:int}")]
        public async Task<IActionResult> RecusarMatricula(int matriculaId)
        {
            await _matriculaService.RecusarMatricula(matriculaId.ToString());
            return NoContent();
        }

        /// <summary>
        /// Obtém a quantidade de alunos ativos vinculados a um professor.
        /// </summary>
        [Authorize(Roles = "professor,administrador")]
        [HttpGet("getAlunosAtivosProfessor/{professorId:int}")]
        public async Task<IActionResult> GetAlunosAtivosProfessor(int professorId)
        {
            var qntdAlunosAtivos = await _matriculaService.GetTotalAlunosAtivosPorProfessor(professorId);
            return Ok(qntdAlunosAtivos);
        }
    }
}
