using GFin.Dados.Models;

namespace GFin.Dados
{
    internal class ChequeResgatadoDAO : GenericDAO<ChequeResgatado>
    {
        internal ChequeResgatadoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
