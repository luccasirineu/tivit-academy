using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using Google.GenAI;

namespace tivitApi.Services
{
    public interface IChatService
    {
        Task<ChatRespostaDTO> ResponderPerguntaAsync(int conteudoId, int alunoId, string pergunta);
        Task<ConteudoContexto> ObterContextoConteudoAsync(int conteudoId);
    }

    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey;
        private const string GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public ChatService(
            AppDbContext context,
            ILogger<ChatService> logger,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
            _geminiApiKey = configuration["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Chave da API Gemini não configurada");
        }

        public async Task<ChatRespostaDTO> ResponderPerguntaAsync(
            int conteudoId,
            int alunoId,
            string pergunta)
        {
            try
            {
                _logger.LogInformation($"Nova pergunta do aluno {alunoId} sobre conteúdo {conteudoId}");

                // Validar que o conteúdo pertence à turma do aluno
                _logger.LogInformation($"Validando acesso ao conteúdo {conteudoId}...");
                var conteudo = await _context.Conteudos
                    .FirstOrDefaultAsync(c => c.Id == conteudoId);

                if (conteudo == null)
                {
                    _logger.LogWarning($"Conteúdo {conteudoId} não encontrado.");
                    return new ChatRespostaDTO
                    {
                        Sucesso = false,
                        Mensagem = "Conteúdo não encontrado",
                        DataResposta = DateTime.UtcNow
                    };
                }

                // Obter contexto
                _logger.LogInformation($"Obtendo contexto para o conteúdo {conteudoId}...");
                var conteudoContexto = await ObterContextoConteudoAsync(conteudoId);

                if (conteudoContexto == null || conteudoContexto.StatusExtracao != "sucesso")
                {
                    _logger.LogWarning($"Contexto do conteúdo {conteudoId} ausente ou falhou na extração. Status: {conteudoContexto?.StatusExtracao}");
                    return new ChatRespostaDTO
                    {
                        Sucesso = false,
                        Mensagem = "Contexto do conteúdo não disponível. Tente novamente mais tarde.",
                        DataResposta = DateTime.UtcNow
                    };
                }

                // Validar que pergunta não está vazia
                _logger.LogInformation("Validando a pergunta enviada...");
                if (string.IsNullOrWhiteSpace(pergunta))
                {
                    _logger.LogWarning("Pergunta não pode estar vazia.");
                    return new ChatRespostaDTO
                    {
                        Sucesso = false,
                        Mensagem = "Pergunta não pode estar vazia",
                        DataResposta = DateTime.UtcNow
                    };
                }

                // Chamar Gemini com contexto + pergunta
                _logger.LogInformation("Enviando dados para o modelo do Gemini...");
                var resposta = await ConsultarGeminiComContextoAsync(
                    conteudoContexto.ContextoTexto,
                    conteudo.Titulo,
                    pergunta);

                _logger.LogInformation($"Resposta gerada com sucesso para a pergunta do aluno {alunoId}.");
                return new ChatRespostaDTO
                {
                    Resposta = resposta,
                    Sucesso = true,
                    Mensagem = "Resposta gerada com sucesso",
                    DataResposta = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao responder pergunta: {ex.Message}");
                return new ChatRespostaDTO
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao processar pergunta: {ex.Message}",
                    DataResposta = DateTime.UtcNow
                };
            }
        }

        public async Task<ConteudoContexto> ObterContextoConteudoAsync(int conteudoId)
        {
            try
            {
                _logger.LogInformation($"Buscando contexto de texto no banco para o conteúdo {conteudoId}...");
                var contexto = await _context.ConteudosContexto
                    .FirstOrDefaultAsync(cc => cc.ConteudoId == conteudoId);

                if (contexto == null)
                {
                    _logger.LogWarning($"Nenhum contexto encontrado no banco para o conteúdo {conteudoId}.");
                }
                else
                {
                    _logger.LogInformation($"Contexto recuperado para o conteúdo {conteudoId}. Status: {contexto.StatusExtracao}");
                }

                return contexto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter contexto: {ex.Message}");
                return null;
            }
        }

        private async Task<string> ConsultarGeminiComContextoAsync(
            string contextoTexto,
            string tituloConteudo,
            string pergunta)
        {
            try
            {
                _logger.LogInformation("Preparando contexto e prompt para a API do Gemini...");
                // Limitar contexto para não exceder limite
                var contextoLimitado = contextoTexto.Length > 15000
                    ? contextoTexto.Substring(0, 15000) + "..."
                    : contextoTexto;

                var prompt = $@"
                    Você é um assistente educacional ajudando um aluno a entender o conteúdo de '{tituloConteudo}'.

                    CONTEXTO DO MATERIAL:
                    {contextoLimitado}

                    PERGUNTA DO ALUNO:
                    {pergunta}

                    Instruções:
                    1. Responda APENAS com informações do contexto fornecido
                    2. Se a pergunta não estiver relacionada ao material, diga que não é possível responder
                    3. Seja claro, conciso e didático
                    4. Use exemplos do material quando apropriado
                    5. Se precisar de informação fora do contexto, mencione isso

                    Responda agora:";

                var client = new Client(apiKey: _geminiApiKey);

                _logger.LogInformation("Realizando requisição HTTP para o modelo gemini-3-flash-preview...");
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3-flash-preview",
                    contents: prompt
                );

                _logger.LogInformation("Resposta do Gemini recebida com sucesso.");
                return (response.Candidates[0].Content.Parts[0].Text);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao consultar Gemini: {ex.Message}");
                throw;
            }
        }
    }
}
