using GFin.Dados.Models;

namespace GFin.Dados
{
    internal class ChequeCompensadoDAO : GenericDAO<ChequeCompensado>
    {
        internal ChequeCompensadoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
