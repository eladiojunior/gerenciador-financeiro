using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class ContaCorrenteDAO : GenericDAO<ContaCorrente>
    {
        internal ContaCorrenteDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
