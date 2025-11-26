namespace tivitApi.DTOs
{
    public class ComprovantePagamentoDTO
    {
        

        public int MatriculaId { get; set; }

        public byte[] Arquivo { get; set; }

        public DateTime HoraEnvio { get; set; }

        public ComprovantePagamentoDTO(int matriculaId, byte[] arquivo, DateTime horaEnvio)
        {
            MatriculaId = matriculaId;
            Arquivo = arquivo;
            HoraEnvio = horaEnvio;
        }

        public ComprovantePagamentoDTO() { }


    }
}
