using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ChequeCanceladoDAO : GenericDAO<ChequeCancelado>
    {
        internal ChequeCanceladoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
