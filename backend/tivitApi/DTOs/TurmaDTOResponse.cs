namespace tivitApi.DTOs
{
    public class TurmaDTOResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int CursoId { get; set; }



        public TurmaDTOResponse()
        {

        }

        public TurmaDTOResponse(int id, string nome, int cursoId)
        {
            Id = id;
            Nome = nome;
            CursoId = cursoId;
        }

    }
}
