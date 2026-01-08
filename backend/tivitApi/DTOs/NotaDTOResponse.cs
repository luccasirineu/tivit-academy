namespace tivitApi.DTOs
{
	public class NotaDTOResponse
	{
		
		public int AlunoId { get; set; }
		public int MateriaId { get; set; }

		public decimal Nota1 { get; set; }
		public decimal Nota2 { get; set; }
		public decimal Media { get; set; }

		public int QtdFaltas { get; set; }
		public string Status { get; set; }

		public NotaDTOResponse(int alunoId, int materiaId, decimal nota1, decimal nota2, int qtdFaltas, string status)
		{
			AlunoId = alunoId;
			MateriaId = materiaId;
			Nota1 = nota1;
			Nota2 = nota2;
			QtdFaltas = qtdFaltas;
			Status = status;

		}

		public NotaDTOResponse() { }



	}
}