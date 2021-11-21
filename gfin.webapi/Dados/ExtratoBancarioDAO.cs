using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ExtratoBancarioDAO : GenericDAO<ExtratoBancarioConta>
    {
        internal ExtratoBancarioDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
