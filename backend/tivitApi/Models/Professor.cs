using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace tivitApi.Models
{
	public class Professor
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public string Nome { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Senha { get; set; }

		[Required]
		public string Rm { get; set; }

        [Required]
        public string Cpf { get; set; }

        [Required]
        public string Status { get; set; }

        public Professor(string nome, string email, string cpf, string senha, string rm)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Senha = senha;
            Rm = rm;
        }



        public Professor() { }
    }
}


	