using GFin.Dados.Models;

namespace GFin.Dados
{
    internal class ChequeDevolvidoDAO : GenericDAO<ChequeDevolvido>
    {
        internal ChequeDevolvidoDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
