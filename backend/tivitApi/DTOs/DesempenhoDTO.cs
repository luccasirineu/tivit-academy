namespace tivitApi.DTOs
{
    public class DesempenhoDTO
    {

        public string NomeMateria { get; set; }
        public decimal Media { get; set; }

        public int QtdFaltas { get; set; }
        public string Nivel{ get; set; }

        public DesempenhoDTO(string nomeMateria, decimal media, int qtdFaltas, string nivel)
        {
            NomeMateria = nomeMateria;
            Media = media;
            QtdFaltas = qtdFaltas;
            Nivel = nivel;

        }

        public DesempenhoDTO() { }



    }
}