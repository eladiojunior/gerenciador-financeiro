using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class NaturezaContaDAO : GenericDAO<NaturezaConta>
    {
        internal NaturezaContaDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
