using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ChequeDevolvidoDAO : GenericDAO<ChequeDevolvido>
    {
        internal ChequeDevolvidoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
