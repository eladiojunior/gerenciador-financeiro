using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ProcessoAutomaticoDAO : GenericDAO<ProcessoAutomatico>
    {
        internal ProcessoAutomaticoDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
