namespace tivitApi.DTOs
{
    public class TurmaDTOResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int CursoId { get; set; }
        public string Status { get; set; }


        public TurmaDTOResponse()
        {

        }

        public TurmaDTOResponse(int id, string nome, int cursoId, string status)
        {
            Id = id;
            Nome = nome;
            CursoId = cursoId;
            Status = status;
        }

    }
}
