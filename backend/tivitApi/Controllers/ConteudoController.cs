using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudoController : ControllerBase
    {
        private readonly IConteudoService _conteudoService;

        public ConteudoController(IConteudoService conteudoService)
        {
            _conteudoService = conteudoService;
        }

        [HttpPost("upload/pdf")]
        public async Task<IActionResult> UploadPdf(
            [FromForm] CreateConteudoPdfDTO dto)
        {
            var professorId = 1;
            var conteudo = await _conteudoService.CriarConteudoPdfAsync(dto, professorId);
            return StatusCode(201, conteudo);
        }

        [HttpPost("upload/link")]
        public async Task<IActionResult> CriarLink(
            [FromBody] CreateConteudoLinkDTO dto)
        {
            var professorId = 1;
            var conteudo = await _conteudoService.CriarConteudoLinkAsync(dto, professorId);
            return StatusCode(201, conteudo);
        }

        [HttpGet("getAllConteudos/{materiaId}/{turmaId}")]
        public async Task<IActionResult> GetConteudo(int materiaId, int turmaId)
        {

            var conteudos = await _conteudoService.GetConteudosByMateriaIdAsync(materiaId, turmaId);

            return Ok(new { conteudos });

        }

    }
}