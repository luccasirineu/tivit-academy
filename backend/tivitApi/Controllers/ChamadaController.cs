using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;
using tivitApi.Exceptions;


namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadaController : ControllerBase
    {
        private readonly IChamadaService _chamadaService;

        public ChamadaController(IChamadaService chamadaService)
        {
            _chamadaService = chamadaService;
        }

        [HttpPost("realizarChamada")]
        public async Task<IActionResult> RealizarChamada([FromBody] List<ChamadaDTO> dtos)
        {

            try
            {
                await _chamadaService.RealizarChamada(dtos);

                return NoContent();

            }
            catch (BusinessException ex)
            {
                return Conflict(new
                {
                    tipo = "CHAMADA_JA_REALIZADA",
                    mensagem = ex.Message
                });
            }
        }

    }
}
