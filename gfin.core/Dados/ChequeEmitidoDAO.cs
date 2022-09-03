using GFin.Dados.Models;

namespace GFin.Dados
{
    internal class ChequeEmitidoDAO : GenericDAO<ChequeEmitido>
    {
        internal ChequeEmitidoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
