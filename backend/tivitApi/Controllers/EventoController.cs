using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;

namespace tivitApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EventoController : ControllerBase
	{
		private readonly IEventoService _eventoService;

		public EventoController(IEventoService eventoService)
		{
			_eventoService = eventoService;
		}


		[HttpPost("adicionarEvento")]
		public async Task<IActionResult> AdicionarEvento([FromBody] EventoDTO eventoDTO)
		{
			var resultado = await _eventoService.criarEvento(eventoDTO);

			if (resultado is string erro)
				return BadRequest(new { sucesso = false, mensagem = erro });

			return Ok(resultado);
		}


	}
}