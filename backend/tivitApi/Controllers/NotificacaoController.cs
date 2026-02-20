using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly INotificacaoService _notificacaoService;

        public NotificacaoController(INotificacaoService notificacaoService)
        {
            _notificacaoService = notificacaoService;
        }

        [HttpPost("criarNotificacao")]
        public async Task<IActionResult> CriarNotificacao([FromBody] NotificacaoDTORequest notificacaoDTO)
        {
            try
            {
                await _notificacaoService.CriarNotificacao(notificacaoDTO);

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
    }
}

