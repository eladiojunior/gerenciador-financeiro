using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class LancamentoExtratoBancarioDAO : GenericDAO<LancamentoExtratoBancario>
    {
        internal LancamentoExtratoBancarioDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
