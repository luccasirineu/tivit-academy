using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class ComprovantePagamento
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
        public byte[] Arquivo { get; set; }

        [Required]
        public DateTime HoraEnvio { get; set; }
       

        public ComprovantePagamento(int matriculaId, byte[] arquivo, DateTime horaEnvio)
        {
            MatriculaId = matriculaId;
            Arquivo = arquivo;
            HoraEnvio = horaEnvio;
        }

    }
}
