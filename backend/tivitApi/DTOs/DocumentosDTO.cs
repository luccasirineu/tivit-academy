

namespace tivitApi.DTOs
{
    public class DocumentosDTO
    {
     
       
        public int MatriculaId { get; set; }

        public byte[] DocumentoHistorico { get; set; }

        public byte[] DocumentoCpf { get; set; }

        public DateTime HoraEnvio { get; set; }


        public DocumentosDTO(int matriculaId, byte[] documentoHistorico, byte[] documentoCpf, DateTime horaEnvio)
        {
            MatriculaId = matriculaId;
            DocumentoHistorico = documentoHistorico;
            DocumentoCpf = documentoCpf;
            HoraEnvio = horaEnvio;
        }

    }
}
