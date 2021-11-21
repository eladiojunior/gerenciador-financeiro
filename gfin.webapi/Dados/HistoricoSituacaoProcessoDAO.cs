using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class HistoricoSituacaoProcessoDAO : GenericDAO<HistoricoSituacaoProcesso>
    {
        internal HistoricoSituacaoProcessoDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
