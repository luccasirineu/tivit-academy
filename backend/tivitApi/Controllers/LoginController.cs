using Microsoft.AspNetCore.Mvc;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Services;
using tivitApi.Exceptions;

namespace tivitApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LoginController : ControllerBase
	{
		private readonly ILoginService _loginService;

		public LoginController(ILoginService loginService)
		{
			_loginService = loginService;
		}

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var resultado = await _loginService.LoginAsync(loginDTO);
                return Ok(resultado);
            }
            catch (RequisicaoInvalidaException ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
            catch (CredenciaisInvalidasException ex)
            {
                return Unauthorized(new { sucesso = false, mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = "Erro interno do servidor" });
            }
        }
    }
}