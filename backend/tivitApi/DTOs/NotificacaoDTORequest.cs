namespace tivitApi.DTOs
{
	public class NotificacaoDTORequest
	{

		public string Titulo{ get; set; }
		public string Descricao { get; set; }
		public List<int> TurmasIds { get; set; }


		public NotificacaoDTORequest(string titulo, string descricao, List<int> turmasIds)
		{
			Titulo = titulo;
			Descricao = descricao;
			TurmasIds = turmasIds;

		}

		public NotificacaoDTORequest() { }



	}
}