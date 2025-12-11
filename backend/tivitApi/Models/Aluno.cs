using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tivitApi.Models
{
    public class Aluno
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
        public string Senha { get; set; }


        [ForeignKey("Matricula")]
        public int MatriculaId { get; set; }
        public Matricula Matricula { get; set; }


        public Aluno(string nome, string email, string cpf, string senha, int matriculaId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Senha = senha;
            MatriculaId = matriculaId;
        }

        

        public Aluno() { }

    }
}
