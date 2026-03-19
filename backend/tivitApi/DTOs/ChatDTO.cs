namespace tivitApi.DTOs
{
    public class ChatPerguntaDTO
    {
        public int ConteudoId { get; set; }
        public string Pergunta { get; set; }
    }

    public class ChatRespostaDTO
    {
        public string Resposta { get; set; }
        public DateTime DataResposta { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class ChatHistoricoDTO
    {
        public int Id { get; set; }
        public int ConteudoId { get; set; }
        public int AlunoId { get; set; }
        public string Pergunta { get; set; }
        public string Resposta { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
