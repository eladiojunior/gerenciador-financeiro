using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class CartaoCreditoDAO : GenericDAO<CartaoCredito>
    {
        internal CartaoCreditoDAO(GFinContext dbContexto) : base(dbContexto) {}
    }
}
