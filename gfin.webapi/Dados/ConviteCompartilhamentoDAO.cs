using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ConviteCompartilhamentoDAO : GenericDAO<ConviteCompartilhamento>
    {
        internal ConviteCompartilhamentoDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
