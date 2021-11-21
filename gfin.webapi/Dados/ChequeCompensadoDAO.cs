using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ChequeCompensadoDAO : GenericDAO<ChequeCompensado>
    {
        internal ChequeCompensadoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
