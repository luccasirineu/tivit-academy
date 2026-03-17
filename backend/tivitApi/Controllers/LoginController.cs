using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tivitApi.DTOs;
using tivitApi.Exceptions;
using tivitApi.Services;

namespace tivitApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILoginService loginService, ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginDTOResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO, CancellationToken cancellationToken)
        {
            if (loginDTO == null)
                return BadRequest(new { sucesso = false, mensagem = "Payload inválido." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(loginDTO.Cpf) || string.IsNullOrWhiteSpace(loginDTO.Senha))
                return BadRequest(new { sucesso = false, mensagem = "CPF e senha são obrigatórios." });

            // validação básica do tipo (ajuste conforme valores aceitos no sistema)
            var tipo = loginDTO.Tipo?.Trim()?.ToLowerInvariant();
            if (string.IsNullOrEmpty(tipo) || (tipo != "aluno" && tipo != "professor" && tipo != "administrador"))
                return BadRequest(new { sucesso = false, mensagem = "Tipo inválido." });

            try
            {
                var resultado = await _loginService.LoginAsync(loginDTO);
                return Ok(resultado);
            }
            catch (RequisicaoInvalidaException ex)
            {
                _logger.LogWarning(ex, "Login inválido (payload) para CPF {Cpf}", MaskCpf(loginDTO.Cpf));
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
            catch (CredenciaisInvalidasException ex)
            {
                // contabilizar tentativa falha / lockout no service
                _logger.LogWarning("Credenciais inválidas para CPF {Cpf}", MaskCpf(loginDTO.Cpf));
                return Unauthorized(new { sucesso = false, mensagem = "Credenciais inválidas." });
            }
            catch (Exception ex)
            {
                // logue o erro completo apenas nos logs; responda mensagem genérica ao cliente
                _logger.LogError(ex, "Erro inesperado ao autenticar CPF {Cpf}", MaskCpf(loginDTO?.Cpf));
                return Problem(detail: "Erro interno ao processar a requisição.", statusCode: 500);
            }
        }

        private static string MaskCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return "N/A";

            var clean = cpf.Replace(".", "").Replace("-", "").Trim();
            if (clean.Length < 4)
                return "***";

            var suffix = clean[^2..]; // últimos 2 dígitos
            return $"***.***.***-{suffix}";
        }
    }
}