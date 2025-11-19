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

        // FK
        [ForeignKey("Professor")]
        public string ProfResponsavel { get; set; }

        public Curso(string nome)
        {
            Nome = nome;
        }

    }
}
