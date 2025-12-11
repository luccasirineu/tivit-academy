namespace tivitApi.DTOs
{

    public class AlunoDTO
    {
        
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public int MatriculaId { get; set; }


        public AlunoDTO(string nome, string email, string cpf, string senha, int matriculaId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Senha = senha;
            MatriculaId = matriculaId;
        }

        public AlunoDTO() { }

    }
}
