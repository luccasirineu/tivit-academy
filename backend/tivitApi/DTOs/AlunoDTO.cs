namespace tivitApi.DTOs
{

    public class AlunoDTO
    {
        public int? AlunoId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public int MatriculaId { get; set; }
        public int TurmaId { get; set; }

        public string? CursoNome { get; set; }

        public AlunoDTO(string nome, string email, string cpf, string senha, int matriculaId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Senha = senha;
            MatriculaId = matriculaId;
        }

        public AlunoDTO(string nome, string email, string cpf, int matriculaId, int alunoId, int turmaId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            MatriculaId = matriculaId;
            AlunoId = alunoId;
            TurmaId = turmaId;
        }

        public AlunoDTO() { }

    }
}
