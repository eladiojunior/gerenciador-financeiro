using GFin.Dados.Models;

namespace GFin.Dados
{
    internal class ContaCorrenteDAO : GenericDAO<ContaCorrente>
    {
        internal ContaCorrenteDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
