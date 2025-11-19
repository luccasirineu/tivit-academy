namespace tivitApi.DTOs
{
	public class MatriculaDTO
	{
		public string Nome { get; set; }
		public string Email { get; set; }
		public string Cpf { get; set; }
		public int CursoId { get; set; }

        public MatriculaDTO(string nome, string email, string cpf, int cursoId)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            CursoId = cursoId;
        }

        public MatriculaDTO() { } 


    }
}
