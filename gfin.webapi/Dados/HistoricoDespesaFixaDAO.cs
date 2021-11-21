using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class HistoricoDespesaFixaDAO : GenericDAO<HistoricoDespesaFixa>
    {
        internal HistoricoDespesaFixaDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
