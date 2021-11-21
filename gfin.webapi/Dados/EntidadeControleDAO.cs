using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class EntidadeControleDAO : GenericDAO<EntidadeControle>
    {
        internal EntidadeControleDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
