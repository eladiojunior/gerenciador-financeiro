namespace gfin.webapi.Api.Models
{
    public class Erro
    {
        public string Atributo { get; set; }
        public string Mensagem { get; set; }

        public Erro(string atributo, string mensagem)
        {
            this.Atributo = atributo;
            this.Mensagem = mensagem;
        } 
    }
}