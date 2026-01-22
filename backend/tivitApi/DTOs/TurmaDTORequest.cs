namespace tivitApi.DTOs
{
    public class TurmaDTORequest
    {
        public string Nome { get; set; }
        public int CursoId { get; set; }
        


        public TurmaDTORequest()
        {

        }

        public TurmaDTORequest(string nome, int cursoId)
        {
            Nome = nome;
            CursoId = cursoId;
        }

    }
}
