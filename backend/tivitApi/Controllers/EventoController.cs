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

		[HttpGet("proximoEvento")]
		public async Task<IActionResult> ProximoEvento()
		{
			var eventoDTO = await _eventoService.obterProximoEvento();


			return Ok(new
			{
				eventoDTO.Titulo,
				eventoDTO.Descricao,
				eventoDTO.Horario
			});
		}

		[HttpGet("getAllEvents")]
		public async Task<IActionResult> GetAllEvents()
		{
			var eventosDTO = await _eventoService.getAllEvents();


			return Ok(eventosDTO);
		}

        [HttpGet("getNextWeekEvents")]
        public async Task<IActionResult> GetNextWeekEvents()
        {
            var qntdEventos = await _eventoService.getNextWeekEvents();


            return Ok(qntdEventos);
        }


    }
}