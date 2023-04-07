using System.Collections.Generic;

namespace GFin.Dados
{
    public class ListaPaginada<T>
    {
        public bool HasProximoPagina { get; set; }
        public bool HasPaginaAnterior { get; set; }
        public long NumeroRegistros { get; set; }
        public long NumeroPaginas { get; set; }
        public List<T> Resultado { get; set; }
    }
}
