using GFin.Dados.Models;
using GFin.Negocio;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace GFin.Dados
{
    internal class ReceitaMensalDAO : GenericDAO<ReceitaMensal>
    {
        internal ReceitaMensalDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
        /// <summary>
        /// Recupera o total de receita de um intervalo de datas (inicial e final), conforme o indicador informado:
        /// - 0 = Total Geral das Receitas;
        /// - 1 = Total de Receitas Recebidas;
        /// - 2 = Total de Receitas Abertas;
        /// - 3 = Total de Receitas Vencidas;
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="dataInicialFiltro">Data da Inicial do filtro da recuperação do total.</param>
        /// <param name="dataFinalFiltro">Data da Final do filtro da recuperação do total.</param>
        /// <param name="indTipoTotalRecebida">Indicador de retorno do valor total de receita.</param>
        /// <returns></returns>
        internal decimal ObterTotalReceita(int idEntidade, DateTime dataInicialFiltro, DateTime dataFinalFiltro, int indTipoTotalRecebida)
        {
            DateTime systemDateTime = GetSystemDateTime();
            IQueryable<ReceitaMensal> query = DbContextoGFin().ReceitaMensal.Where(d => d.IdEntidade == idEntidade && d.DataRecebimentoReceita >= dataInicialFiltro && d.DataRecebimentoReceita <= dataFinalFiltro);
            switch (indTipoTotalRecebida)
            {
                case 1: //Todas as receitas Recebidas;
                    query = query.Where(d => d.IsReceitaLiquidada == true);
                    break;
                case 2: //Todas as receitas Abertas;
                    query = query.Where(d => d.IsReceitaLiquidada == false && d.DataRecebimentoReceita >= systemDateTime);
                    break;
                case 3: //Todas as receitas Vencidas;
                    query = query.Where(d => d.IsReceitaLiquidada == false && d.DataRecebimentoReceita < systemDateTime);
                    break;
            }
            decimal totalReceita = query.Sum(d => (decimal?)d.ValorReceita) ?? 0;
            return totalReceita;
        }

        /// <summary>
        /// Lista de receita mensal por filtro.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param> 
        /// <param name="filtro">Filtro com as informações para recuperação das receitas.</param>
        /// <returns></returns>
        internal List<ReceitaMensal> ListarReceitaPorFiltro(int idEntidade, Negocio.Filtros.FiltroReceitaMensal filtro)
        {

            IQueryable<ReceitaMensal> query = DbContextoGFin().ReceitaMensal.AsQueryable();
            
            query = query.Include(rm => rm.NaturezaContaReceita);
            
            query = query.Where(rm => rm.IdEntidade == idEntidade &&
                rm.DataRecebimentoReceita >= filtro.DataInicialFiltro &&
                rm.DataRecebimentoReceita <= filtro.DataFinalFiltro);

            if (!filtro.HasTodas && !filtro.HasRecebidas)
            {
                query = query.Where(rm => rm.IsReceitaLiquidada == filtro.HasRecebidas);
            }
            query = query.OrderByAsc(rm => rm.DataRecebimentoReceita);

            return query.ToList();
        }

        /// <summary>
        /// Recupera os totais de receitas anuais do ano informado.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="anoCorrente">Ano para totalizar as receitas.</param>
        /// <returns></returns>
        internal Dictionary<int, decimal> ListarTotaisReceitaAnual(int idEntidade, int anoCorrente)
        {
            Dictionary<int, decimal> listaTotais = new Dictionary<int, decimal>();
            DateTime dataInicialAnoCorrente = new DateTime(anoCorrente, 1, 1);
            DateTime dataFinalAnoCorrente = new DateTime(anoCorrente, 12, 31);
            var result = DbContextoGFin().ReceitaMensal.Where(d => d.IdEntidade == idEntidade &&
                d.DataRecebimentoReceita >= dataInicialAnoCorrente &&
                d.DataRecebimentoReceita <= dataFinalAnoCorrente)
                  .GroupBy(g => g.DataRecebimentoReceita.Month)
                  .Select(g => new {
                      Mes = g.Key,
                      Total = g.Sum(i => i.ValorReceita)
                  });
            foreach (var item in result.ToList())
            {
                listaTotais.Add(item.Mes, item.Total);
            }
            return listaTotais;
        }

        /// <summary>
        /// Recupera a lista de totais de receitas vencidas nos meses anteriores a data informada.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="dataBuscaAnterior">Data para recuperação dos totais das receitas vencidas.</param>
        /// <returns></returns>
        internal List<ReceitaMensal> ListarTotaisReceitasVencidas(int idEntidade, DateTime dataBuscaAnterior)
        {
            var queryTotais =
                (from d in DbContextoGFin().ReceitaMensal
                 where d.IdEntidade == idEntidade &&
                     d.IsReceitaLiquidada == false &&
                     d.DataRecebimentoReceita < dataBuscaAnterior
                 group d by new { d.DataRecebimentoReceita.Month, d.DataRecebimentoReceita.Year }
                     into grp
                     select new
                     {
                         Qtd = grp.Count(),
                         grp.Key.Month,
                         grp.Key.Year,
                         Total = grp.Sum(d => d.ValorReceita)
                     }).ToList();

            var listaVencidas = new List<ReceitaMensal>();
            foreach (var item in queryTotais)
            {
                ReceitaMensal receita = new ReceitaMensal();
                receita.IdEntidade = idEntidade;
                receita.DataRecebimentoReceita = new DateTime(item.Year, item.Month, 1);
                receita.TextoDescricaoReceita = string.Format("[{0}] Receita vencidas de {1}/{2}", item.Qtd, UtilNegocio.ObterNomeMes(item.Month), item.Year);
                receita.ValorReceita = item.Total;
                listaVencidas.Add(receita);
            }
            return listaVencidas;

        }
    }
}
