using GFin.Dados;
using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
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
