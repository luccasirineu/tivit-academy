
namespace tivitApi.DTOs
{
	public class EventoDTO
	{
		public int Id { get; set; }

		public string Titulo{ get; set; }

		public string Descricao{ get; set; }

		public DateTime Horario { get; set; }


		public EventoDTO(int id, string titulo, string descricao, DateTime horario)
		{
			Id = id;
			Titulo = titulo;
			Descricao = descricao;
			Horario = horario;
		}

		public EventoDTO(string titulo, string descricao, DateTime horario)
		{
			Titulo = titulo;
			Descricao = descricao;
			Horario = horario;
		}

		public EventoDTO()
		{
			
		}
	}
}
