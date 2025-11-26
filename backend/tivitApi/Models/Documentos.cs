using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Documentos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // FK 
        [ForeignKey("Matricula")]
        public int MatriculaId { get; set; }

        // Navigation property
        public Matricula Matricula { get; set; }

        [Required]
        public byte[] DocumentoHistorico { get; set; }

        [Required]
        public byte[] DocumentoCpf { get; set; }

        [Required]
        public DateTime HoraEnvio { get; set; }


        public Documentos(int matriculaId, byte[] documentoHistorico, byte[] documentoCpf, DateTime horaEnvio)
        {
            MatriculaId = matriculaId;
            DocumentoHistorico = documentoHistorico;
            DocumentoCpf = documentoCpf;
            HoraEnvio = horaEnvio;
        }

    }
}
