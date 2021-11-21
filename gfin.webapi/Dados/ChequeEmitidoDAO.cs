using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ChequeEmitidoDAO : GenericDAO<ChequeEmitido>
    {
        internal ChequeEmitidoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
