using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class ConteudoContexto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Conteudo")]
        public int ConteudoId { get; set; }
        public Conteudo Conteudo { get; set; }

        [Required]
        public string ContextoTexto { get; set; }

        [Required]
        public DateTime DataArmazenamento { get; set; }

        public string StatusExtracao { get; set; } = "sucesso";

        public string? MensagemErro { get; set; }

        [Required]
        public int TurmaId { get; set; }
    }
}
