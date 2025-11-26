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

        // Navigation property
        public Professor Professor { get; set; }

        public Curso(string nome)
        {
            Nome = nome;
        }

    }
}
