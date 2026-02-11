using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("getUserByCpf")]
        public async Task<IActionResult> GetUserByCpf([FromQuery] string cpf)
        {
            try
            {
                var user = await _userService.GetUserByCpf(cpf);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }

        [HttpGet("getUsersByNome")]
        public async Task<IActionResult> GetUsersByNome([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("Nome do usuário é obrigatório.");

            try
            {
                var users = await _userService.GetUsersByNome(nome);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("desativar")]
        public async Task<IActionResult> DesativarUser([FromQuery] string cpf, [FromQuery] string tipo)
        {

            try
            {
                await _userService.DesativarUser(cpf, tipo);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpPut("ativar")]
        public async Task<IActionResult> AtivarUser([FromQuery] string cpf, [FromQuery] string tipo)
        {

            try
            {
                await _userService.AtivarUser(cpf, tipo);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }
    }
}