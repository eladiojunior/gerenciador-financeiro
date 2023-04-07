using GFin.Dados.Models;

namespace GFin.Dados
{
    internal class ChequeCanceladoDAO : GenericDAO<ChequeCancelado>
    {
        internal ChequeCanceladoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
