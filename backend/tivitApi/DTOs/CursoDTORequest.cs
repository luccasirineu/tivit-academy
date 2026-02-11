namespace tivitApi.DTOs
{
    public class CursoDTORequest
    {

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public int ProfResponsavel { get; set; }

        public string Status { get; set; }

        public CursoDTORequest(string nome, string descricao, int profResponsavel, string status)
        {
            Nome = nome;
            Descricao = descricao;
            ProfResponsavel = profResponsavel;
            Status = status;
        }

        public CursoDTORequest() { }


    }
}
