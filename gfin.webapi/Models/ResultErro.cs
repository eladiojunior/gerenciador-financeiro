using System.Collections.Generic;

namespace gfin.webapi.Api.Models
{
    public class ResultErro
    {
        private List<Erro> _erros;

        private ResultErro()
        {
            _erros = new List<Erro>();
        }
        public static ResultErro Get()
        {
            return new ResultErro();
        }

        public List<Erro> AddErro(string atributo, string mensagem)
        {
            _erros.Add(new Erro(atributo, mensagem));
            return _erros;
        }
        public List<Erro> AddErro(string mensagem)
        {
            _erros.Add(new Erro(null, mensagem));
            return _erros;
        }

        public List<Erro> Erros()
        {
            return _erros;
        }
    }
}