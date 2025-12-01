using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Matricula
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Cpf { get; set; }

        [Required]
        public string Status{ get; set; }

        // FK
        [ForeignKey("Curso")]
        public int CursoId { get; set; }

        public Curso curso { get; set; }


        public Matricula(string nome, string email, string cpf, int cursoId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Status = "AGUARDANDO_PAGAMENTO";
            CursoId = cursoId;
        }

        public Matricula(string nome, string email, string cpf, string status, int cursoId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Status = status;
            CursoId = cursoId;
        }

        public Matricula() { }

    }
}
