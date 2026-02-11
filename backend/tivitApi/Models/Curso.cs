using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Curso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Descricao { get; set; }

        // FK 
        [ForeignKey("Professor")]
        public int ProfResponsavel { get; set; }
        public Professor Professor { get; set; }

        public string Status { get; set; }

        public Curso(string nome, string descricao, int profResponsavel, string status)
        {
            Nome = nome;
            Descricao = descricao;
            ProfResponsavel = profResponsavel;
            Status = status;
        }

    }
}
