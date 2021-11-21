using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gfin.webapi.Dados
{
    internal class DespesaMensalDAO : GenericDAO<DespesaMensal>
    {
        internal DespesaMensalDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }

        /// <summary>
        /// Recupera o total de despesa de um intervalo de datas (inicial e final), conforme o indicador informado:
        /// - 1 = Total de Despesas Pagas;
        /// - 2 = Total de Despesas Abertas;
        /// - 3 = Total de Despesas Vencidas;
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="dataInicialFiltro">Data da Inicial do filtro da recuperação do total.</param>
        /// <param name="dataFinalFiltro">Data da Final do filtro da recuperação do total.</param>
        /// <param name="indTipoTotalDespesa">Indicador de retorno do valor total de despesa.</param>
        /// <returns></returns>
        internal decimal ObterTotalDespesa(int idEntidade, DateTime dataInicialFiltro, DateTime dataFinalFiltro, int indTipoTotalDespesa)
        {
            DateTime systemDateTime = GetSystemDateTime();
            decimal totalDespesa = 0;
            IQueryable<DespesaMensal> query = DbContextoGFin().DespesaMensal.Where(d => d.IdEntidade == idEntidade && d.DataVencimentoDespesa >= dataInicialFiltro && d.DataVencimentoDespesa <= dataFinalFiltro);
            switch (indTipoTotalDespesa)
            {
                case 1: //Todas as despesas Pagas;
                    {
                        query = query.Where(d => d.IsDespesaLiquidada == true);
                        totalDespesa = query.Sum(d => (decimal?)d.ValorTotalLiquidacaoDespesa) ?? 0;
                        break;
                    }
                case 2: //Todas as despesas Abertas;
                    {
                        query = query.Where(d => d.IsDespesaLiquidada == false && d.DataVencimentoDespesa >= systemDateTime);
                        totalDespesa = query.Sum(d => (decimal?)d.ValorDespesa) ?? 0;
                        break;
                    }
                case 3: //Todas as despesas Vencidas;
                    {
                        query = query.Where(d => d.IsDespesaLiquidada == false && d.DataVencimentoDespesa < systemDateTime);
                        totalDespesa = query.Sum(d => (decimal?)d.ValorDespesa) ?? 0;
                        break;
                    }
            }
            
            return totalDespesa;
        }

        /// <summary>
        /// Lista de despesa mensal por filtro.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="filtro">Filtro com as informações para recuperação das despesas.</param>
        /// <returns></returns>
        internal List<DespesaMensal> ListarDespesaPorFiltro(int idEntidade, Negocio.Filtros.FiltroDespesaMensal filtro)
        {
            
            IQueryable<DespesaMensal> query = DbContextoGFin().DespesaMensal.AsQueryable();
            
            query = query.Include(dm => dm.NaturezaContaDespesa);
            
            query = query.Where(d => d.IdEntidade == idEntidade && 
                d.DataVencimentoDespesa >= filtro.DataInicialVencimento && 
                d.DataVencimentoDespesa <= filtro.DataFinalVencimento);
            
            if (!filtro.HasTodas && !filtro.HasPagas)
            {
                query = query.Where(d => d.IsDespesaLiquidada == filtro.HasPagas);
            }

            query = query.OrderByAsc(dm => dm.DataVencimentoDespesa);

            return query.ToList();
        }

        /// <summary>
        /// Recupera os totais de despesas anuais do ano informado.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="anoCorrente">Ano para totalizar as despesas.</param>
        /// <returns></returns>
        internal Dictionary<int, decimal> ListarTotaisDespesasAnual(int idEntidade, int anoCorrente)
        {
            Dictionary<int, decimal> listaTotais = new Dictionary<int, decimal>();
            DateTime dataInicialAnoCorrente = new DateTime(anoCorrente, 1, 1);
            DateTime dataFinalAnoCorrente = new DateTime(anoCorrente, 12, 31);
            var result = DbContextoGFin().DespesaMensal.Where(d => d.IdEntidade == idEntidade &&
                d.DataVencimentoDespesa >= dataInicialAnoCorrente &&
                d.DataVencimentoDespesa <= dataFinalAnoCorrente)
                  .GroupBy(g => g.DataVencimentoDespesa.Month)
                  .Select(g => new {
                      Mes = g.Key,
                      Total = g.Sum(i => i.ValorDespesa)
                  });
            foreach (var item in result.ToList())
            {
                listaTotais.Add(item.Mes, item.Total);
            }
            return listaTotais;
        }

        /// <summary>
        /// Recupera a lista de totais de despesas vencidas nos meses anteriores a data informada.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="dataBuscaAnterior">Data para recuperação dos totais das despesas vencidas.</param>
        /// <returns></returns>
        internal List<DespesaMensal> ListarTotaisDespesasVencidas(int idEntidade, DateTime dataBuscaAnterior)
        {

            var queryTotais = 
                (from d in DbContextoGFin().DespesaMensal
                    where d.IdEntidade == idEntidade && 
                        d.IsDespesaLiquidada == false && 
                        d.DataVencimentoDespesa < dataBuscaAnterior
                    group d by new {d.DataVencimentoDespesa.Month, d.DataVencimentoDespesa.Year}
                    into grp
                        select new
                        {
                            Qtd = grp.Count(),
                            grp.Key.Month,
                            grp.Key.Year,
                            Total = grp.Sum(d => d.ValorDespesa)
                        }).ToList();

            var listaDespesasVencidas = new List<DespesaMensal>();
            foreach (var item in queryTotais)
            {
                DespesaMensal despesa = new DespesaMensal();
                despesa.IdEntidade = idEntidade;
                despesa.DataVencimentoDespesa = new DateTime(item.Year, item.Month, 1);
                despesa.DescricaoDespesa = string.Format("[{0}] Despesas vencidas de {1}/{2}", item.Qtd, UtilNegocio.ObterNomeMes(item.Month), item.Year);
                despesa.ValorDespesa = item.Total;
                listaDespesasVencidas.Add(despesa);
            }
            return listaDespesasVencidas;

        }
    }
}
