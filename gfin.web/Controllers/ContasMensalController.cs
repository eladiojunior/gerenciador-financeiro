using GFin.Negocio;
using GFin.Negocio.Filtros;
using GFin.Web.Controllers.Helpers;
using GFin.Web.Models;
using GFin.Web.Models.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class ContasMensalController : GenericController
    {

        //
        // GET: /ContasMensal/Index
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Verifica na Dropbox o value informado (dataValue) para seleção no componente.
        /// </summary>
        /// <param name="dataValue">Data para seleção do dropbox;</param>
        /// <param name="dropboxModel">Componente dropbox que será procurado no value.</param>
        private void CarregarDropboxFiltroMesAno(DateTime dataValue, Models.Helpers.DropboxModel dropboxModel)
        {
            foreach (var item in dropboxModel.Itens)
            {
                item.Selected = item.Value.Equals(dataValue.ToString("dd/MM/yyyy"));
                if (item.Selected) break;
            }
        }

        //
        // GET /ContasMensal/JsonListarContasMensal
        [HttpGet]
        public JsonResult JsonListarContasMensal(string dataFiltro, bool hasExibirReceitas, bool hasExibirDespesas, bool hasExibirLiquidadas, bool hasExibirAbertasMesesAnteriores)
        {
            try
            {

                FiltroContasModel model = new FiltroContasModel();
                model.IsExibirLiquidadas = hasExibirLiquidadas;
                model.IsExibirDespesas = hasExibirDespesas;
                model.IsExibirReceitas = hasExibirReceitas;
                model.IsExibirContasAbertasMesesAnteriores = hasExibirAbertasMesesAnteriores;
                if (string.IsNullOrEmpty(dataFiltro))
                {
                    model.DataInicialFiltro = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                }
                else
                {
                    DateTime dataFiltroReceita = DateTime.Parse(dataFiltro);
                    model.DataInicialFiltro = dataFiltroReceita;
                }
                model.DropboxFiltroMesAno = DropboxHelper.DropboxMesesDoAno(model.DataInicialFiltro.Year);
                CarregarDropboxFiltroMesAno(model.DataInicialFiltro, model.DropboxFiltroMesAno);
                model.ContasMensais = ListarContas(model);
                ObterTotaisContas(model);

                return JsonResultSucesso(RenderRazorViewToString("_ListaContasPartial", model));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonListarContasMensal(dataFiltro::{0})", dataFiltro)));
            }
        }

        /// <summary>
        /// Totalizadores no filtro de contas (despesa/receitas).
        /// </summary>
        /// <param name="filtroModel"></param>
        private void ObterTotaisContas(FiltroContasModel filtroModel)
        {
            var dataInicialVencimento = filtroModel.DataInicialFiltro;
            var dataFinalVencimento = new DateTime(filtroModel.DataInicialFiltro.Year, filtroModel.DataInicialFiltro.Month, UtilNegocio.ObterDiasMes(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year), 23, 59, 59);

            //Recuperar o totalizador das Despesas...
            var negocioDespesa = new DespesaNegocio(UsuarioLogadoConfig.Instance);
            filtroModel.ValorTotalDespesas = negocioDespesa.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 0); //Total Geral
            filtroModel.ValorTotalDespesasLiquidadas = negocioDespesa.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 1); //Total Liquidadas
            filtroModel.ValorTotalDespesasAbertas = negocioDespesa.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 2); //Total Abertas
            filtroModel.ValorTotalDespesasVencidas = negocioDespesa.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 3); //Total Vencidas

            //Recuperar o totalizador das Receitas...
            var negocioReceita = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
            filtroModel.ValorTotalReceitas = negocioReceita.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 0); //Total Geral
            filtroModel.ValorTotalReceitasLiquidadas = negocioReceita.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 1); //Total Recebidas
            filtroModel.ValorTotalReceitasAbertas = negocioReceita.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 2); //Total Abertas
            filtroModel.ValorTotalReceitasVencidas = negocioReceita.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 3); //Total Vencidas

        }

        /// <summary>
        /// Recupera a lista de contas (despesas/receitas) (ativas) do negócio.
        /// </summary>
        /// <returns></returns>
        private List<Core.Negocio.DTOs.ContasMensalDTO> ListarContas(FiltroContasModel filtroModel)
        {
            List<Core.Negocio.DTOs.ContasMensalDTO> listContas = new List<Core.Negocio.DTOs.ContasMensalDTO>();

            if (filtroModel.IsExibirDespesas)
            {
                var negocioDespesa = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                var filtroDespesa = new FiltroDespesaMensal();
                filtroDespesa.DataInicialVencimento = filtroModel.DataInicialFiltro;
                filtroDespesa.DataFinalVencimento = new DateTime(filtroModel.DataInicialFiltro.Year, filtroModel.DataInicialFiltro.Month, UtilNegocio.ObterDiasMes(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year));
                filtroDespesa.HasAbertas = true;
                filtroDespesa.HasPagas = filtroModel.IsExibirLiquidadas;
                filtroDespesa.HasVencidas = true;
                var listDespesas = negocioDespesa.ListarDespesaMensal(filtroDespesa);
                foreach (var item in listDespesas)
                {
                    Core.Negocio.DTOs.ContasMensalDTO conta = new Core.Negocio.DTOs.ContasMensalDTO();
                    conta.IdConta = item.Id;
                    conta.DescricaoNaturezaConta = item.NaturezaContaDespesa.DescricaoNaturezaConta;
                    conta.DescricaoConta = item.DescricaoDespesa;
                    conta.DataConta = item.DataVencimentoDespesa;
                    conta.DescricaoTipoFormaLiquidacao = Dados.Enums.UtilEnum.GetTextoFormaLiquidacao(item.CodigoTipoFormaLiquidacao);
                    conta.ValorConta = item.ValorDespesa;
                    conta.IsContaLiquidada = item.IsDespesaLiquidada;
                    if (conta.IsContaLiquidada)
                    {
                        conta.ValorConta = item.ValorTotalLiquidacaoDespesa.Value;
                        conta.DataLiquidacaoConta = item.DataHoraLiquidacaoDespesa;
                    }
                    int qtdDiasVencimento = VerificarContaVencida(item.DataVencimentoDespesa);
                    conta.IsContaVencida = (qtdDiasVencimento != 0);
                    conta.QtdDiasVencimento = qtdDiasVencimento;
                    conta.CodigoTipoConta = "D"; //Despesa;
                    listContas.Add(conta);
                }
                if (filtroModel.IsExibirContasAbertasMesesAnteriores)
                {//Recuperar lista de despesas (abertas) de meses anteriores...
                    var listDespesasMesesAnteriores = negocioDespesa.ListarTotaisDespesasVencidasMesesAnteriores(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year);
                    foreach (var item in listDespesasMesesAnteriores)
                    {
                        Core.Negocio.DTOs.ContasMensalDTO conta = new Core.Negocio.DTOs.ContasMensalDTO();
                        conta.IdConta = 0;
                        conta.DescricaoNaturezaConta = "-";
                        conta.DescricaoConta = item.DescricaoDespesa;
                        conta.DataConta = item.DataVencimentoDespesa;
                        conta.DescricaoTipoFormaLiquidacao = "-";
                        conta.ValorConta = item.ValorDespesa;
                        conta.IsContaLiquidada = false;
                        int qtdDiasVencimento = VerificarContaVencida(item.DataVencimentoDespesa);
                        conta.IsContaVencida = (qtdDiasVencimento != 0);
                        conta.QtdDiasVencimento = qtdDiasVencimento;
                        conta.CodigoTipoConta = "D"; //Despesa;
                        listContas.Add(conta);
                    }
                }
            }

            if (filtroModel.IsExibirReceitas)
            {
                var negocioReceita = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                var filtroReceita = new FiltroReceitaMensal();
                filtroReceita.DataInicialFiltro = filtroModel.DataInicialFiltro;
                filtroReceita.DataFinalFiltro = new DateTime(filtroModel.DataInicialFiltro.Year, filtroModel.DataInicialFiltro.Month, UtilNegocio.ObterDiasMes(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year));
                filtroReceita.HasAbertas = true;
                filtroReceita.HasRecebidas = filtroModel.IsExibirLiquidadas;
                filtroReceita.HasVencidas = true;
                var listReceitas = negocioReceita.ListarReceitaMensal(filtroReceita);
                foreach (var item in listReceitas)
                {
                    Core.Negocio.DTOs.ContasMensalDTO conta = new Core.Negocio.DTOs.ContasMensalDTO();
                    conta.IdConta = item.Id;
                    conta.DescricaoNaturezaConta = item.NaturezaContaReceita.DescricaoNaturezaConta;
                    conta.DescricaoConta = item.TextoDescricaoReceita;
                    conta.DataConta = item.DataRecebimentoReceita;
                    conta.DescricaoTipoFormaLiquidacao = Dados.Enums.UtilEnum.GetTextoFormaRecebimento(item.CodigoTipoFormaRecebimento);
                    conta.IsContaLiquidada = item.IsReceitaLiquidada;
                    conta.ValorConta = item.ValorReceita;
                    if (conta.IsContaLiquidada)
                    {
                        conta.ValorConta = item.ValorTotalLiquidacaoReceita.Value;
                        conta.DataLiquidacaoConta = item.DataHoraLiquidacaoReceita;
                    }
                    int qtdDiasVencimento = VerificarContaVencida(item.DataRecebimentoReceita);
                    conta.IsContaVencida = (qtdDiasVencimento != 0);
                    conta.QtdDiasVencimento = qtdDiasVencimento;
                    conta.CodigoTipoConta = "R"; //Receita;
                    listContas.Add(conta);
                }
                if (filtroModel.IsExibirContasAbertasMesesAnteriores)
                {//Recuperar lista de Receitas (abertas) de meses anteriores...
                    var listDespesasMesesAnteriores = negocioReceita.ListarTotaisReceitasVencidasMesesAnteriores(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year);
                    foreach (var item in listDespesasMesesAnteriores)
                    {
                        Core.Negocio.DTOs.ContasMensalDTO conta = new Core.Negocio.DTOs.ContasMensalDTO();
                        conta.IdConta = 0;
                        conta.DescricaoNaturezaConta = "-";
                        conta.DescricaoConta = item.TextoDescricaoReceita;
                        conta.DataConta = item.DataRecebimentoReceita;
                        conta.DescricaoTipoFormaLiquidacao = "-";
                        conta.ValorConta = item.ValorReceita;
                        conta.IsContaLiquidada = false;
                        int qtdDiasVencimento = VerificarContaVencida(item.DataRecebimentoReceita);
                        conta.IsContaVencida = (qtdDiasVencimento != 0);
                        conta.QtdDiasVencimento = qtdDiasVencimento;
                        conta.CodigoTipoConta = "R"; //Receita;
                        listContas.Add(conta);
                    }

                }
            }

            return listContas.OrderBy(o => o.DataConta).ToList();
        }

        /// <summary>
        /// Verifica se a conta encontra-se vencida, pela data da conta.
        /// </summary>
        /// <param name="dataConta">Data a conta (despesa ou receita) para verificar se está vencida.</param>
        /// <returns>Retorna a quantidade de dias que a conta encontra-se vencida, caso não esteja retorna 0 (zero);</returns>
        private int VerificarContaVencida(DateTime dataConta)
        {
            int qtdDiasAtraso = 0;
            if (dataConta.CompareTo(DateTime.Now.Date) == -1)
            {
                qtdDiasAtraso = (short)(DateTime.Now.Date - dataConta).TotalDays;
            }
            return qtdDiasAtraso;
        }
    }
}
