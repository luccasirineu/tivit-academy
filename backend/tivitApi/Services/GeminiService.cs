using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using tivitApi.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Google.GenAI;

namespace tivitApi.Services
{
    public interface IGeminiService
    {
        Task<ConteudoContexto> ExtrairEProcessarConteudoAsync(
            Conteudo conteudo,
            string caminhoOuUrl,
            string tipoConteudo);
    }

    public class GeminiService : IGeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey;

        public GeminiService(
            IConfiguration configuration,
            ILogger<GeminiService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;

            _geminiApiKey = _configuration["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Chave da API Gemini não configurada");
        }

        public async Task<ConteudoContexto> ExtrairEProcessarConteudoAsync(
            Conteudo conteudo,
            string caminhoOuUrl,
            string tipoConteudo)
        {
            try
            {
                _logger.LogInformation($"[Início] Processando ConteudoId: {conteudo.Id} | Tipo: {tipoConteudo}");

                // 1. Extração
                _logger.LogInformation($"[Extração] Iniciando extração de texto de: {caminhoOuUrl}");
                var textoExtraido = await ExtrairTextoAsync(caminhoOuUrl, tipoConteudo);

                if (string.IsNullOrWhiteSpace(textoExtraido))
                {
                    _logger.LogWarning($"[Extração] Falha. Texto vazio obtido de: {caminhoOuUrl}");
                    throw new Exception("Texto vazio após extração");
                }
                
                _logger.LogInformation($"[Extração] Sucesso. Tamanho do texto extraído: {textoExtraido.Length} caracteres.");

                // 2. Contexto IA 
                _logger.LogInformation($"[Contexto IA] Iniciando geração de contexto com Gemini (Título: '{conteudo.Titulo}')");
                var contexto = await GerarContextoComGeminiAsync(textoExtraido, conteudo.Titulo);
                _logger.LogInformation($"[Contexto IA] Sucesso. Tamanho do contexto gerado: {contexto.Length} caracteres.");


                _logger.LogInformation($"[Fim] Processamento do ConteudoId: {conteudo.Id} concluído com sucesso.");

                return new ConteudoContexto
                {
                    ConteudoId = conteudo.Id,
                    ContextoTexto = contexto, 
                    DataArmazenamento = DateTime.UtcNow,
                    StatusExtracao = "sucesso",
                    TurmaId = conteudo.TurmaId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Erro] Falha no processamento do ConteudoId: {conteudo.Id}");

                return new ConteudoContexto
                {
                    ConteudoId = conteudo.Id,
                    ContextoTexto = "",
                    DataArmazenamento = DateTime.UtcNow,
                    StatusExtracao = "erro",
                    MensagemErro = ex.Message,
                    TurmaId = conteudo.TurmaId
                };
            }
        }

 

        private async Task<string> ExtrairTextoAsync(string caminho, string tipo)
        {
            if (tipo.ToLower() == "pdf")
                return await ExtrairTextoDePdfAsync(caminho);

            throw new Exception($"Tipo não suportado: {tipo}");
        }

        private async Task<string> ExtrairTextoDePdfAsync(string caminho)
        {
            // Lê todo o arquivo para bytes em memória primeiro
            var bytes = await File.ReadAllBytesAsync(caminho);

            return await Task.Run(() =>
            {
                var sb = new StringBuilder();
                using (var ms = new MemoryStream(bytes))
                using (var doc = new PdfDocument(new PdfReader(ms)))
                {
                    for (int i = 1; i <= doc.GetNumberOfPages(); i++)
                    {
                        var page = doc.GetPage(i);
                        var strategy = new SimpleTextExtractionStrategy();
                        var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                        sb.AppendLine(text);
                    }
                }
                return sb.ToString();
            });
        }

        private async Task<string> GerarContextoComGeminiAsync(string texto, string titulo)
        {
        
            var prompt = $@"
                Você é um assistente educacional.

                Com base no conteúdo abaixo sobre '{titulo}', gere:

                1. Conceitos principais
                2. Definições
                3. Exemplos
                4. Relações
                5. Resumo

                Conteúdo:
                {texto}
                ";

            var client = new Client(apiKey: _geminiApiKey);

            var response = await client.Models.GenerateContentAsync(
                model: "gemini-3-flash-preview",
                contents: prompt
            );


            return (response.Candidates[0].Content.Parts[0].Text);
        }



    }
}