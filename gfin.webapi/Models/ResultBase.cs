namespace gfin.webapi.Api.Models
{
    public class ResultBase
    {
        public bool Status { get; set; }
        public string Mensagem { get; set; }
        public object Data { get; set; }
    }
}