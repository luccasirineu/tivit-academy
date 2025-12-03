namespace tivitApi.DTOs
{
	public class MatriculaDTO
	{
        public int Id { get; set; }
        public string Nome { get; set; }
		public string Email { get; set; }
		public string Cpf { get; set; }
        public string? Status { get; set; }
        public int CursoId { get; set; }

        public MatriculaDTO(string nome, string email, string cpf, int cursoId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Status = "AGUARDANDO_PAGAMENTO";
            CursoId = cursoId;
        }

        public MatriculaDTO(string nome, string email, string cpf, string status, int cursoId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Status = status;
            CursoId = cursoId;
        }

        public MatriculaDTO(int id, string nome, string email, string cpf, string status, int cursoId)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Cpf = cpf;
            Status = status;
            CursoId = cursoId;
        }

        public MatriculaDTO() { } 


    }
}
