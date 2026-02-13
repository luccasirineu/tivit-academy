namespace tivitApi.DTOs
{
    public class TurmaDTORequest
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int CursoId { get; set; }
        public string Status { get; set; }


        public TurmaDTORequest()
        {

        }

        public TurmaDTORequest(string nome, int cursoId, string status)
        {
            Nome = nome;
            CursoId = cursoId;
            Status = status;
        }

        public TurmaDTORequest(int id, string nome, int cursoId, string status)
        {
            Id = id;
            Nome = nome;
            CursoId = cursoId;
            Status = status;
        }

    }
}
