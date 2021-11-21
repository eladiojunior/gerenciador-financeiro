using gfin.webapi.Dados.Models;
using System.Linq;

namespace gfin.webapi.Dados
{
    internal class DespesaFixaDAO : GenericDAO<DespesaFixa>
    {
        internal DespesaFixaDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
        /// <summary>
        /// Recupera o total de despesas fixas, vigentes, pela entidade selecionada.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade.</param>
        internal decimal ObterTotalDespesasFixas(int idEntidade)
        {
            decimal totalDespesasFixas = DbContextoGFin().DespesaFixa
                .Where(s => s.IdEntidade == idEntidade && s.CodigoTipoSituacaoDespesaFixa == (int)Dados.Enums.TipoSituacaoEnum.Ativo)
                .Sum(s => (decimal?)s.ValorDespesaFixa) ?? 0;
            return totalDespesasFixas;
        }
    }
}
