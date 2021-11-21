using gfin.webapi.Dados.Models;
using System.Linq;

namespace gfin.webapi.Dados
{
    internal class ReceitaFixaDAO : GenericDAO<ReceitaFixa>
    {
        internal ReceitaFixaDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
        /// <summary>
        /// Recupera o total de receitas fixas, vigentes, pela entidade selecionada.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade.</param>
        internal decimal ObterTotalReceitasFixas(int idEntidade)
        {
            decimal totalReceitasFixas = DbContextoGFin().ReceitaFixa
                .Where(s => s.IdEntidade == idEntidade && s.CodigoTipoSituacaoReceitaFixa == (int)Dados.Enums.TipoSituacaoEnum.Ativo)
                .Sum(s => (decimal?)s.ValorReceitaFixa) ?? 0;
            return totalReceitasFixas;
        }

    }
}
