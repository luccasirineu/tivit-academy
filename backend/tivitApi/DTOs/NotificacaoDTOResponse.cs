namespace tivitApi.DTOs
{
    public class NotificacaoDTOResponse
    {

        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }


        public NotificacaoDTOResponse(string titulo, string descricao, DateTime dataCriacao)
        {
            Titulo = titulo;
            Descricao = descricao;
            DataCriacao = dataCriacao;

        }

        public NotificacaoDTOResponse() { }



    }
}