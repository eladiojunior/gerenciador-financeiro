using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Negocio.Filtros;
using GFin.Web.Controllers.Helpers;
using GFin.Web.Models;
using GFin.Web.Models.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class DespesaMensalController : GenericController
    {
        //
        // GET: /DespesaMensal/

        public ActionResult Index()
        {
            DespesaMensalModel model = new DespesaMensalModel();
            model.DataVencimentoDespesa = DateTime.Now.Date;
            CarregarModelDefault(model);
            return View(model);
        }

        private void CarregarModelDefault(DespesaMensalModel model)
        {
            model.DropboxFormaLiquidacao = DropboxHelper.DropboxFormaLiquidacao();
            model.DropboxNaturezaDespesa = DropboxHelper.DropboxNaturezaDespesa();
            model.FiltroDespesas = CarregarModelFiltroComLista(model.FiltroDespesas);
        }

        private Models.Filtros.FiltroDespesaModel CarregarModelFiltroComLista(Models.Filtros.FiltroDespesaModel model)
        {
            if (model == null)
            {
                model = new Models.Filtros.FiltroDespesaModel();
                model.DataInicialFiltro = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                model.IsFiltroTodas = true;
                model.DropboxFiltroMesAno = DropboxHelper.DropboxMesesDoAno(DateTime.Now.Year);
                SetValueDropboxFiltroMesAno(model.DataInicialFiltro, model.DropboxFiltroMesAno);
            }
            model.DespesasMensais = ListarDespesas(model);
            ObterTotaisDespesa(model);
            return model;
        }

        /// <summary>
        /// Verifica na Dropbox o value informado (dataValue) para seleção no componente.
        /// </summary>
        /// <param name="dataValue">Data para seleção do dropbox;</param>
        /// <param name="dropboxModel">Componente dropbox que será procurado no value.</param>
        private void SetValueDropboxFiltroMesAno(DateTime dataValue, Models.Helpers.DropboxModel dropboxModel)
        {
            foreach (var item in dropboxModel.Itens)
            {
                item.Selected = item.Value.Equals(dataValue.ToString("dd/MM/yyyy"));
                if (item.Selected) break;
            }
        }

        private void ObterTotaisDespesa(FiltroDespesaModel filtroModel)
        {

            var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);

            var dataInicialVencimento = filtroModel.DataInicialFiltro;
            var dataFinalVencimento = new DateTime(filtroModel.DataInicialFiltro.Year, filtroModel.DataInicialFiltro.Month, UtilNegocio.ObterDiasMes(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year), 23, 59, 59);

            filtroModel.ValorTotalDespesa = negocio.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 0); //Total Geral
            filtroModel.ValorTotalDespesaPagas = negocio.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 1); //Total Pagas
            filtroModel.ValorTotalDespesaAbertas = negocio.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 2); //Total Abertas
            filtroModel.ValorTotalDespesaVencidas = negocio.ObterTotalDespesa(dataInicialVencimento, dataFinalVencimento, 3); //Total Vencidas

        }

        /// <summary>
        /// Recupera a lista de despesas (ativas) do negócio.
        /// </summary>
        /// <returns></returns>
        private List<Dados.Models.DespesaMensal> ListarDespesas(FiltroDespesaModel filtroModel)
        {
            var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
            var filtro = new FiltroDespesaMensal();
            filtro.DataInicialVencimento = filtroModel.DataInicialFiltro;
            filtro.DataFinalVencimento = new DateTime(filtroModel.DataInicialFiltro.Year, filtroModel.DataInicialFiltro.Month, UtilNegocio.ObterDiasMes(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year));
            if (filtroModel.IsFiltroTodas)
                filtro.HasTodas = filtroModel.IsFiltroTodas;
            else
            {
                filtro.HasPagas = filtroModel.IsFiltroPagas;
                filtro.HasAbertas = filtroModel.IsFiltroAbertas;
                filtro.HasVencidas = filtroModel.IsFiltroVencidas;
            }
            return negocio.ListarDespesaMensal(filtro);
        }

        //
        // GET: /DespesaMensal/FiltrarDespesas
        [HttpGet]
        public ActionResult FiltrarDespesas(string dataFiltro)
        {

            FiltroDespesaModel model = new FiltroDespesaModel();

            try
            {
                if (string.IsNullOrEmpty(dataFiltro))
                {
                    model = CarregarModelFiltroComLista(null);
                }
                else
                {
                    DateTime dataFiltroDespesa = DateTime.Parse(dataFiltro);
                    model.IsFiltroTodas = true;
                    model.DataInicialFiltro = dataFiltroDespesa;
                    model.DropboxFiltroMesAno = DropboxHelper.DropboxMesesDoAno(dataFiltroDespesa.Year);
                    SetValueDropboxFiltroMesAno(dataFiltroDespesa, model.DropboxFiltroMesAno);
                    model.DespesasMensais = ListarDespesas(model);
                    ObterTotaisDespesa(model);
                }
            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Filtro de Despesas Mensais.");
                model = CarregarModelFiltroComLista(null);
            }

            return PartialView("_ListaDespesaPartial", model);

        }

        //
        // POST: /DespesaMensal/Registrar
        [HttpPost]
        public ActionResult Registrar(DespesaMensalModel model)
        {

            try
            {

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                string strLiquidacao = string.Empty;

                var despesaMensal = new DespesaMensal();
                despesaMensal.IdNaturezaContaDespesa = model.IdNaturezaDespesaMensal;
                despesaMensal.CodigoTipoFormaLiquidacao = model.CodigoFormaPagamentoDespesaMensal;
                despesaMensal.CodigoVinculoFormaLiquidacao = (model.IdVinculoFormaLiquidacao.HasValue ? model.IdVinculoFormaLiquidacao.Value : 0);
                despesaMensal.IsDespesaParcelada = model.IsDespesaMensalParcelada;
                despesaMensal.DescricaoDespesa = model.TextoDescricaoDespesaMensal;
                despesaMensal.DataVencimentoDespesa = model.DataVencimentoDespesa;
                despesaMensal.ValorDespesa = model.ValorDespesa;
                if (model.IsDespesaLiquidada)
                {//Registrar uma despesa já liquidada.
                    despesaMensal.IsDespesaLiquidada = model.IsDespesaLiquidada;
                    despesaMensal.DataHoraLiquidacaoDespesa = model.DataLiquidacaoDespesa;
                    despesaMensal.ValorDescontoLiquidacaoDespesa = model.ValorDescontoLiquidacaoDespesa;
                    despesaMensal.ValorMultaJurosLiquidacaoDespesa = model.ValorMultaJurosLiquidacaoDespesa;
                    despesaMensal.ValorTotalLiquidacaoDespesa = negocio.CalcularTotalDespesaLiquidada(despesaMensal.ValorDespesa, despesaMensal.ValorDescontoLiquidacaoDespesa, despesaMensal.ValorMultaJurosLiquidacaoDespesa);
                    despesaMensal.TextoObservacaoDespesa = model.TextoObservacaoLiquidacaoDespesa;
                    strLiquidacao = " (já liquidada)";
                }

                if (despesaMensal.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.ChequeAVista ||
                    despesaMensal.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.ChequePreDatado)
                {//Verificar se o número do cheque existe para recupera seu ID, caso não exista, criaremos um cheque com o número informado e vincularemos a despesa.
                    ChequeNegocio negocioCheque = new ChequeNegocio(UsuarioLogadoConfig.Instance);
                    Cheque chequeRegistrado = negocioCheque.ObterCheque(model.IdBancoAgenciaContaCorrente, model.IdVinculoFormaLiquidacao.Value);
                    if (chequeRegistrado == null || chequeRegistrado.Id == 0)
                    {//Numero do cheque não identificado... criar
                        chequeRegistrado = new Cheque();
                        chequeRegistrado.IdContaCorrente = model.IdBancoAgenciaContaCorrente;
                        chequeRegistrado.NumeroCheque = model.IdVinculoFormaLiquidacao.Value;
                        chequeRegistrado = negocioCheque.RegistrarCheque(chequeRegistrado);
                    }
                    despesaMensal.CodigoVinculoFormaLiquidacao = chequeRegistrado.Id;
                }

                negocio.RegistrarDespesa(despesaMensal);

                AlertaUsuario(string.Format("Registro de despesa mensal{0} efetuado com sucesso.", strLiquidacao), TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Despesa Mensal");
                CarregarModelDefault(model);
                return View("Index", model);
            }

        }

        //
        // POST: /DespesaMensal/RegistrarDespesaParcelada
        [HttpPost]
        public ActionResult RegistrarDespesaParcelada(DespesaMensalModel model)
        {

            try
            {

                var despesaMensalParcelada = new DespesaMensalParcelada();
                despesaMensalParcelada.IdNaturezaContaDespesa = model.IdNaturezaDespesaMensal;
                despesaMensalParcelada.CodigoTipoFormaLiquidacao = model.CodigoFormaPagamentoDespesaMensal;
                despesaMensalParcelada.TextoDescricaoDespesa = model.TextoDescricaoDespesaMensal;
                despesaMensalParcelada.ParcelamentoDespesa = new List<ParcelaDespesa>();

                string mensagemValidacao = string.Empty;
                if (despesaMensalParcelada.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.ChequePreDatado)
                {//Verificar se exitem numeração de cheques repetidos...
                    var query = from parcelas in model.ParcelasDespesa
                                group parcelas.NumeroParcela by parcelas.NumeroParcela into grouped
                                where grouped.Count() > 1
                                select new { NumeroCheque = grouped.Key, Qtd = grouped.Count() };
                    if (query.Count() > 0)
                    {
                        string numerosCheques = String.Join(", ", query.Select(s => s.NumeroCheque.Value));
                        mensagemValidacao = string.Format("Existem números de cheque repetidos [{0}] no parcelamento, registro de despesa parcelada não realizada.", numerosCheques);
                        AlertaUsuario(mensagemValidacao, TipoAlertaEnum.Erro);
                        return RedirectToAction("Index");
                    }
                }

                bool isChequesValidos = true;
                int numeroParcela = 1;
                int numeroCheque = 0;
                foreach (var item in model.ParcelasDespesa)
                {
                    ParcelaDespesa parcela = new ParcelaDespesa();
                    parcela.CodigoVinculoFormaLiquidacao = (item.IdVinculoFormaLiquidacao.HasValue ? item.IdVinculoFormaLiquidacao.Value : 0);
                    parcela.NumeroParcela = numeroParcela++;
                    parcela.DataVencimentoParcela = item.DataVencimento;
                    parcela.ValorParcela = item.ValorParcela;
                    numeroCheque = item.NumeroParcela.Value;
                    if (despesaMensalParcelada.CodigoTipoFormaLiquidacao == (int)TipoFormaLiquidacaoEnum.ChequePreDatado)
                    {//Verificar se o número do cheque existe para recupera seu ID, caso não exista, criaremos um cheque com o número informado e vincularemos a despesa.
                        ChequeNegocio negocioCheque = new ChequeNegocio(UsuarioLogadoConfig.Instance);
                        Cheque chequeRegistrado = negocioCheque.ObterCheque(model.IdBancoAgenciaContaCorrente, numeroCheque);
                        if (chequeRegistrado == null || chequeRegistrado.Id == 0)
                        {//Numero do cheque não identificado... criar
                            chequeRegistrado = new Cheque();
                            chequeRegistrado.IdContaCorrente = model.IdBancoAgenciaContaCorrente;
                            chequeRegistrado.NumeroCheque = numeroCheque;
                            chequeRegistrado = negocioCheque.RegistrarCheque(chequeRegistrado);
                        }
                        else
                        {//Verificar se o cheque já não foi utilizado.
                            if (chequeRegistrado.CodigoSituacaoCheque != (short)TipoSituacaoChequeEnum.ChequeRegistrado)
                            {
                                isChequesValidos = false;
                                if (!string.IsNullOrEmpty(mensagemValidacao))
                                    mensagemValidacao += ", ";
                                mensagemValidacao += string.Format("[{0}::{1}]", chequeRegistrado.NumeroCheque, UtilEnum.GetTextoTipoSituacaoCheque(chequeRegistrado.CodigoSituacaoCheque));
                            }
                        }
                        parcela.CodigoVinculoFormaLiquidacao = chequeRegistrado.Id;
                    }
                    despesaMensalParcelada.ParcelamentoDespesa.Add(parcela); ;
                }

                if (!isChequesValidos)
                {//Identificado cheque em situação que não podem ser registrado a despesa.
                    mensagemValidacao = "Existe(m) cheque(s) em situação que não permitem o registro da despesa parcelada.<br/>" + mensagemValidacao;
                    AlertaUsuario(mensagemValidacao, TipoAlertaEnum.Erro);
                    CarregarModelDefault(model);
                    return View("Index", model);
                }

                var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RegistrarDespesaParcelada(despesaMensalParcelada);

                AlertaUsuario(string.Format("Foram registradas todas as ({0}) parcelas da despesa mensal.", despesaMensalParcelada.ParcelamentoDespesa.Count), TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Despesa Mensal");
                AlertaUsuario("Erro ao registrar despesa parcelada.", TipoAlertaEnum.Erro);
                CarregarModelDefault(model);
                return View("Index", model);
            }

        }

        //
        //POST: /DespesaMensal/DespesaChequePre
        [HttpPost]
        public ActionResult DespesaChequePre(DespesaMensalModel model)
        {

            DespesaMensalChequeModel modelResult = new DespesaMensalChequeModel();

            try
            {

                modelResult.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrente();
                modelResult.IdBancoAgenciaContaCorrente = model.IdBancoAgenciaContaCorrente;
                modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoFormaLiquidacao(model.CodigoFormaPagamentoDespesaMensal);
                modelResult.DataVencimentoDespesa = model.DataVencimentoDespesa;
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível carregar as informações da despesa mensal.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_RegistrarDespesaChequePrePartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaParceladaChequePre
        [HttpPost]
        public ActionResult DespesaParceladaChequePre(DespesaMensalModel model)
        {

            DespesaMensalParcelamentoChequePreModel modelResult = new DespesaMensalParcelamentoChequePreModel();

            try
            {
                int qtdParcelas = model.QtdParcelasDespesa.HasValue ? model.QtdParcelasDespesa.Value : 0;

                modelResult.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrente();
                if (qtdParcelas != 0)
                {
                    modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoEnum(TipoFormaLiquidacaoEnum.ChequePreDatado);
                    modelResult.QtdParcelasDespesa = qtdParcelas;
                    modelResult.ParcelasDespesa = ParcelarDespesa(qtdParcelas, model.DataVencimentoDespesa, model.ValorDespesa, false);
                }
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível realizar o parcelamento da despesa mensal.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_DespesaParceladaChequePrePartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaCartaoCredito
        [HttpPost]
        public ActionResult DespesaCartaoCredito(int idContaCorrente)
        {

            DespesaMensalCartaoCreditoModel modelResult = new DespesaMensalCartaoCreditoModel();

            try
            {
                modelResult.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrenteCartao();
                modelResult.IdBancoAgenciaContaCorrente = idContaCorrente;
                var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
                modelResult.Cartoes = negocio.ListarCartaoCredito(idContaCorrente, Dados.Enums.TipoSituacaoCartaoCreditoEnum.Ativo);
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível montar tela de registro de despesa mensal por Cartão de Crédito.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_RegistrarDespesaCartaoCreditoPartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaDebitoConta
        [HttpPost]
        public ActionResult DespesaDebitoConta(DespesaMensalModel model)
        {

            DespesaMensalDebitoContaModel modelResult = new DespesaMensalDebitoContaModel();

            try
            {
                modelResult.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrenteCartao(false);
                modelResult.IdBancoAgenciaContaCorrente = model.IdBancoAgenciaContaCorrente;
                modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoFormaLiquidacao(model.CodigoFormaPagamentoDespesaMensal);
                modelResult.DataVencimento = model.DataVencimentoDespesa;
                modelResult.ValorDespesa = model.ValorDespesa;
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível montar tela de registro de despesa mensal por Débito em Conta Corrente.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_RegistrarDespesaDebitoContaPartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaChequeAVista
        [HttpPost]
        public ActionResult DespesaChequeAVista(DespesaMensalModel model)
        {

            DespesaMensalChequeModel modelResult = new DespesaMensalChequeModel();

            try
            {

                modelResult.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrente();
                modelResult.IdBancoAgenciaContaCorrente = model.IdBancoAgenciaContaCorrente;
                modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoFormaLiquidacao(model.CodigoFormaPagamentoDespesaMensal);
                modelResult.DataVencimentoDespesa = model.DataVencimentoDespesa;
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível carregar as informações da despesa mensal.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_RegistrarDespesaChequeAVistaPartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaParceladaCartaoCredito
        [HttpPost]
        public ActionResult DespesaParceladaCartaoCredito(DespesaMensalModel model)
        {

            DespesaMensalParcelamentoCartaoCreditoModel modelResult = new DespesaMensalParcelamentoCartaoCreditoModel();

            try
            {

                int qtdParcelas = model.QtdParcelasDespesa.HasValue ? model.QtdParcelasDespesa.Value : 0;

                modelResult.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrenteCartao();
                if (qtdParcelas != 0)
                {
                    modelResult.IdBancoAgenciaContaCorrente = model.IdBancoAgenciaContaCorrente;
                    var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
                    modelResult.Cartoes = negocio.ListarCartaoCredito(model.IdBancoAgenciaContaCorrente, Dados.Enums.TipoSituacaoCartaoCreditoEnum.Ativo);
                    modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoFormaLiquidacao(model.CodigoFormaPagamentoDespesaMensal);
                    modelResult.QtdParcelasDespesa = qtdParcelas;
                    modelResult.ParcelasDespesa = ParcelarDespesa(qtdParcelas, model.DataVencimentoDespesa, model.ValorDespesa);
                }
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível realizar o parcelamento da despesa mensal.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_DespesaParceladaCartaoCreditoPartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaParceladaBoleto
        [HttpPost]
        public ActionResult DespesaParceladaBoleto(DespesaMensalModel model)
        {

            DespesaMensalParcelamentoBoletoModel modelResult = new DespesaMensalParcelamentoBoletoModel();

            try
            {
                int qtdParcelas = model.QtdParcelasDespesa.HasValue ? model.QtdParcelasDespesa.Value : 0;

                if (qtdParcelas != 0)
                {
                    modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoEnum(TipoFormaLiquidacaoEnum.BoletoCobranca);
                    modelResult.QtdParcelasDespesa = qtdParcelas;
                    modelResult.ParcelasDespesa = ParcelarDespesa(qtdParcelas, model.DataVencimentoDespesa, model.ValorDespesa);
                    model.ParcelasDespesa = null;
                }
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível realizar o parcelamento da despesa mensal.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_DespesaParceladaBoletoPartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaParceladaDebitoConta
        [HttpPost]
        public ActionResult DespesaParceladaDebitoConta(DespesaMensalModel model)
        {

            DespesaMensalParcelamentoDebitoContaModel modelResult = new DespesaMensalParcelamentoDebitoContaModel();

            try
            {

                int qtdParcelas = model.QtdParcelasDespesa.HasValue ? model.QtdParcelasDespesa.Value : 0;

                modelResult.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrenteCartao(false);
                if (qtdParcelas != 0)
                {
                    modelResult.IdBancoAgenciaContaCorrente = model.IdBancoAgenciaContaCorrente;
                    modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoFormaLiquidacao(model.CodigoFormaPagamentoDespesaMensal);
                    modelResult.QtdParcelasDespesa = qtdParcelas;
                    modelResult.ParcelasDespesa = ParcelarDespesa(qtdParcelas, model.DataVencimentoDespesa, model.ValorDespesa);
                }
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível realizar o parcelamento da despesa mensal.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_DespesaParceladaDebitoContaPartial", modelResult);

        }

        //
        //POST: /DespesaMensal/DespesaParceladaFatura
        [HttpPost]
        public ActionResult DespesaParceladaFatura(DespesaMensalModel model)
        {

            DespesaMensalParcelamentoFaturaModel modelResult = new DespesaMensalParcelamentoFaturaModel();

            try
            {

                int qtdParcelas = model.QtdParcelasDespesa.HasValue ? model.QtdParcelasDespesa.Value : 0;
                if (qtdParcelas != 0)
                {
                    modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoFormaLiquidacao(model.CodigoFormaPagamentoDespesaMensal);
                    modelResult.QtdParcelasDespesa = qtdParcelas;
                    modelResult.ParcelasDespesa = ParcelarDespesa(qtdParcelas, model.DataVencimentoDespesa, model.ValorDespesa);
                }
            }
            catch (Exception erro)
            {
                string msg = "Desculpe, não foi possível realizar o parcelamento da despesa mensal.";
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_DespesaParceladaFaturaPartial", modelResult);

        }

        //
        //POST: /DespesaMensal/JsonDespesaParceladaDinheiro
        [HttpPost]
        public JsonResult JsonRegistrarDespesaParceladaDinheiro(DespesaMensalModel model)
        {
            try
            {
                DespesaMensalParcelamentoDinheiroModel modelResult = new DespesaMensalParcelamentoDinheiroModel();
                int qtdParcelas = model.QtdParcelasDespesa.HasValue ? model.QtdParcelasDespesa.Value : 0;
                if (qtdParcelas != 0)
                {
                    modelResult.IdNaturezaDespesaMensal = model.IdNaturezaDespesaMensal;
                    modelResult.CodigoFormaPagamentoDespesaMensal = model.CodigoFormaPagamentoDespesaMensal;
                    modelResult.FormaPagamentoDespesaMensal = UtilEnum.GetTextoEnum(TipoFormaLiquidacaoEnum.Dinheiro);
                    modelResult.TextoDescricaoDespesaMensal = model.TextoDescricaoDespesaMensal;
                    modelResult.QtdParcelasDespesa = qtdParcelas;
                    modelResult.ParcelasDespesa = ParcelarDespesa(qtdParcelas, model.DataVencimentoDespesa, model.ValorDespesa);
                }
                return JsonResultSucesso(RenderRazorViewToString("_DespesaParceladaDinheiroPartial", modelResult));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRegistrarDespesaParceladaDinheiro(DespesaMensalModel)"));
            }
        }

        //GET: /DespesaMensal/JsonDetalharDespesaLiquidada
        [HttpGet]
        public JsonResult JsonDetalharDespesaLiquidada(int idDespesa)
        {

            try
            {

                if (idDespesa == 0)
                    return JsonResultErro("Identificador da despesa mensal não informado.");

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                DespesaMensal despesa = negocio.ObterDespesaMensal(idDespesa);
                if (despesa == null)
                    return JsonResultErro(string.Format("Despesa mensal com o Id [{0}] não encontrada.", idDespesa));

                DetalheDespesaLiquidadaModel modelResult = new DetalheDespesaLiquidadaModel();
                if (despesa != null && despesa.Id != 0)
                {
                    modelResult.IdDespesaMensal = despesa.Id;
                    modelResult.DescricaoNaturezaDespesaMensal = despesa.NaturezaContaDespesa.DescricaoNaturezaConta;
                    modelResult.DescricaoTipoFormaLiquidacao = UtilEnum.GetTextoFormaLiquidacao(despesa.CodigoTipoFormaLiquidacao);
                    modelResult.TextoDescricaoDespesaMensal = despesa.DescricaoDespesa;
                    modelResult.DataVencimentoDespesa = despesa.DataVencimentoDespesa.ToString("dd/MM/yyyy");
                    modelResult.ValorDespesa = despesa.ValorDespesa.ToString("N");
                    modelResult.DataLiquidacaoDespesa = despesa.DataHoraLiquidacaoDespesa.HasValue ? despesa.DataHoraLiquidacaoDespesa.Value.ToString("dd/MM/yyyy") : "";
                    modelResult.ValorDescontoLiquidacaoDespesa = despesa.ValorDescontoLiquidacaoDespesa.HasValue ? despesa.ValorDescontoLiquidacaoDespesa.Value.ToString("N") : ((decimal)0).ToString("N");
                    modelResult.ValorMultaJurosLiquidacaoDespesa = despesa.ValorMultaJurosLiquidacaoDespesa.HasValue ? despesa.ValorMultaJurosLiquidacaoDespesa.Value.ToString("N") : ((decimal)0).ToString("N");
                    modelResult.ValorTotalLiquidacaoDespesa = despesa.ValorTotalLiquidacaoDespesa.HasValue ? despesa.ValorTotalLiquidacaoDespesa.Value.ToString("N") : ((decimal)0).ToString("N");
                    modelResult.TextoObservacaoLiquidacaoDespesa = despesa.TextoObservacaoDespesa;
                }
                return JsonResultSucesso(RenderRazorViewToString("_DetalharDespesaLiquidadaPartial", modelResult));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonDetalharDespesaLiquidada(DespesaMensalModel)"));
            }

        }

        //POST: /DespesaMensal/JsonEstornarDespesaLiquidada
        [HttpPost]
        public JsonResult JsonEstornarDespesaLiquidada(int idDespesa)
        {

            try
            {
                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                negocio.EstornarDespesaMensal(idDespesa);
                return JsonResultSucesso("Despesa mensal estornada com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonEstornarDespesaLiquidada(DespesaMensalModel)"));
            }

        }

        /// <summary>
        /// Realizar o parcelamento da despesa mensal GERAL
        /// </summary>
        /// <param name="qtdParcelas">Quantidade de parcelas</param>
        /// <param name="dataVencimento">Data incial do parcelamento.</param>
        /// <param name="valorParcela">Valor da Parcela</param>
        /// <param name="isNumerarParcelas">Indicador de numeração da parcela, caso true, será retornada os números das parcelas.</param>
        /// <returns></returns>
        private List<DespesaMensalParcelaModel> ParcelarDespesa(int qtdParcelas, DateTime dataVencimento, decimal valorParcela, bool isNumerarParcelas = true)
        {
            List<DespesaMensalParcelaModel> listParcelamento = new List<DespesaMensalParcelaModel>();
            DateTime dataVencimentoParcela = dataVencimento;
            for (int i = 1; i <= qtdParcelas; i++)
            {
                DespesaMensalParcelaModel despesaParcelada = new DespesaMensalParcelaModel();
                if (isNumerarParcelas)
                    despesaParcelada.NumeroParcela = i;
                despesaParcelada.DataVencimento = dataVencimentoParcela;
                dataVencimentoParcela = UtilNegocio.ObterProximaData(dataVencimentoParcela, 30, true);
                despesaParcelada.ValorParcela = valorParcela;
                listParcelamento.Add(despesaParcelada);
            }
            return listParcelamento;
        }

        //
        // GET: /DespesaMensal/JsonParcelamentoChequePre
        [HttpGet]
        public JsonResult JsonParcelamentoChequePre(int idContaCorrente, int numeroChequeInicial, int qtdParcelas)
        {
            string mensagemErro = string.Empty;
            try
            {
                if (idContaCorrente == 0 || numeroChequeInicial == 0 || qtdParcelas == 0)
                {
                    mensagemErro = "Desculpe, não foi possível recuperar os números dos cheque do parcelamento.";
                    return Json(new { HasErro = true, MensagemErro = mensagemErro }, JsonRequestBehavior.AllowGet);
                }

                //Recupera Conta Corrente.
                List<SituacaoChequeModel> listaSituacaoCheque = new List<SituacaoChequeModel>();
                ContaCorrenteNegocio contaCorrenteNegocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
                ContaCorrente conta = contaCorrenteNegocio.ObterContaCorrente(idContaCorrente);
                if (conta != null)
                {
                    for (int numeroCheque = numeroChequeInicial; numeroCheque < (numeroChequeInicial + qtdParcelas); numeroCheque++)
                    {
                        listaSituacaoCheque.Add(ObterSituacaoCheque(idContaCorrente, numeroCheque));
                    }
                }
                return Json(new { HasErro = false, Data = listaSituacaoCheque }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception erro)
            {
                mensagemErro = TratarMensagemErroNegocio(erro, "JsonParcelamentoChequePre(idContaCorrente, numeroChequeInicial, qtdParcelas)");
                return Json(new { HasErro = true, MensagemErro = mensagemErro }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: /DespesaMensal/JsonVerificarNumeroCheque
        [HttpGet]
        public JsonResult JsonVerificarNumeroCheque(int idContaCorrente, int numeroCheque)
        {
            string mensagemErro = string.Empty;
            try
            {
                if (idContaCorrente == 0 || numeroCheque == 0)
                {
                    mensagemErro = "Desculpe, não foi possível verificar o número do cheque.";
                    return Json(new { HasErro = true, MensagemErro = mensagemErro }, JsonRequestBehavior.AllowGet);
                }

                //Recupera Conta Corrente.
                SituacaoChequeModel situacaoCheque = new SituacaoChequeModel();
                ContaCorrenteNegocio contaCorrenteNegocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
                ContaCorrente conta = contaCorrenteNegocio.ObterContaCorrente(idContaCorrente);
                if (conta != null)
                {
                    situacaoCheque = ObterSituacaoCheque(idContaCorrente, numeroCheque);
                }
                return Json(new { HasErro = false, Data = situacaoCheque }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception erro)
            {
                mensagemErro = TratarMensagemErroNegocio(erro, "JsonVerificarNumeroCheque(idContaCorrente, numeroCheque)");
                return Json(new { HasErro = true, MensagemErro = mensagemErro }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Recupera a situação do Cheque, pelo número informado.
        /// </summary>
        /// <param name="idContaCorrente">Identificador da conta corrente que o cheque pertence.</param>
        /// <param name="numeroCheque">Numero do cheque que será consultado.</param>
        /// <returns></returns>
        private SituacaoChequeModel ObterSituacaoCheque(int idContaCorrente, int numeroCheque)
        {
            SituacaoChequeModel situacaoCheque = new SituacaoChequeModel();
            situacaoCheque.NumeroCheque = numeroCheque;

            ChequeNegocio chequeNegocio = new ChequeNegocio(UsuarioLogadoConfig.Instance);
            Cheque cheque = chequeNegocio.ObterCheque(idContaCorrente, numeroCheque);
            if (cheque != null)
            {
                situacaoCheque.IdCheque = cheque.Id;
                situacaoCheque.CodigoSituacaoCheque = cheque.CodigoSituacaoCheque;
                situacaoCheque.DescicaoSituacaoCheque = UtilEnum.GetTextoTipoSituacaoCheque(cheque.CodigoSituacaoCheque);
                situacaoCheque.IsUtilizavel = (cheque.CodigoSituacaoCheque == 1);
            }
            else
            {
                situacaoCheque.IdCheque = 0;
                situacaoCheque.CodigoSituacaoCheque = 0; //Não Registrado
                situacaoCheque.DescicaoSituacaoCheque = "Cheque não registrado, será registrado automaticamente.";
                situacaoCheque.IsUtilizavel = true; //Não registrado;
            }
            return situacaoCheque;
        }

        //
        // GET: /DespesaMensal/Editar/{id}
        public ActionResult Editar(int id)
        {
            DespesaMensalModel modelIndex = new DespesaMensalModel();
            CarregarModelDefault(modelIndex);

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id da Despesa Mensal não informado.");
                return View("Index", modelIndex);
            }

            var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
            DespesaMensal despesa = negocio.ObterDespesaMensal(id);
            if (despesa == null || despesa.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Despesa Mensal com Id [{0}] não encontrada.", id));
                return View("Index", modelIndex);
            }
            if (despesa.IsDespesaLiquidada)
            {
                ModelState.AddModelError(string.Empty, string.Format("Despesa mensal já liquidada em [{0}] não pode ser editada.", despesa.DataHoraLiquidacaoDespesa.Value.ToString("dd/MM/yyyy")));
                return View("Index", modelIndex);
            }

            DespesaMensalEdicaoModel model = new DespesaMensalEdicaoModel();
            model.DropboxNaturezaDespesa = DropboxHelper.DropboxNaturezaDespesa();

            model.IdDespesaMensal = despesa.Id;
            model.IdNaturezaDespesaMensal = despesa.IdNaturezaContaDespesa;
            model.TextoDescricaoDespesaMensal = despesa.DescricaoDespesa;
            model.CodigoFormaPagamentoDespesaMensal = despesa.CodigoTipoFormaLiquidacao;
            model.DescricaoTipoFormaLiquidacao = Dados.Enums.UtilEnum.GetTextoFormaLiquidacao(despesa.CodigoTipoFormaLiquidacao);
            model.IsDespesaMensalParcelada = despesa.IsDespesaParcelada;
            if (despesa.IsDespesaParcelada) {
                //Separar o numero da parcela da descrição, para facilitar a edição do usuário...
                //Só será realizado essa separação quando a despesa for parcelada...
                int idxTraco = despesa.DescricaoDespesa.IndexOf("-");
                if (idxTraco > 1)
                {
                    string numeroParcelaDespesa = despesa.DescricaoDespesa.Substring(0, idxTraco);
                    model.TextoDescricaoDespesaMensal = despesa.DescricaoDespesa.Substring(idxTraco + 1);
                    model.NumeroParcelaDespesa = numeroParcelaDespesa;
                }
                short qtdParcelas = (short)negocio.ObterQtdParcelasDespesa(despesa.CodigoDespesaParcelada.Value);
                model.QtdParcelasDespesa = qtdParcelas;
            }
            model.DataVencimentoDespesa = despesa.DataVencimentoDespesa;
            model.ValorDespesa = despesa.ValorDespesa;

            return View(model);

        }

        //
        // POST: /DespesaMensal/Editar
        [HttpPost]
        public ActionResult Editar(DespesaMensalEdicaoModel model)
        {

            model.DropboxNaturezaDespesa = DropboxHelper.DropboxNaturezaDespesa();
            if (!ModelState.IsValid)
            {
                return View("Editar", model);
            }

            try
            {

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                var despesaMensal = negocio.ObterDespesaMensal(model.IdDespesaMensal);
                if (despesaMensal == null)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Despesa Mensal com Id [{0}] não encontrada.", model.IdDespesaMensal));
                    return View("Editar", model);
                }

                if (despesaMensal.IdNaturezaContaDespesa != model.IdNaturezaDespesaMensal)
                    despesaMensal.NaturezaContaDespesa = null;
                despesaMensal.IdNaturezaContaDespesa = model.IdNaturezaDespesaMensal;
                despesaMensal.DescricaoDespesa = model.TextoDescricaoDespesaMensal;
                if (despesaMensal.IsDespesaParcelada)
                {
                    despesaMensal.DescricaoDespesa = string.Format("{0}-{1}", model.NumeroParcelaDespesa, model.TextoDescricaoDespesaMensal);
                }
                despesaMensal.DataVencimentoDespesa = model.DataVencimentoDespesa;
                despesaMensal.ValorDespesa = model.ValorDespesa;

                negocio.GravarDespesaMensal(despesaMensal);

                AlertaUsuario("Despesa mensal alterada com sucesso.", TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Editar Despesa Mensal");
                return View("Editar", model);
            }

        }

        //
        // GET /DespesaMensal/LiquidarDespesaMensal
        [HttpGet]
        public JsonResult JsonLiquidarDespesaMensal(int idDespesa)
        {

            try
            {

                if (idDespesa == 0)
                    return JsonResultErro("Identificador da despesa mensal não informado.");

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                DespesaMensal despesa = negocio.ObterDespesaMensal(idDespesa);
                if (despesa == null)
                    return JsonResultErro(string.Format("Despesa mensal com o Id [{0}] não encontrada.", idDespesa));

                LiquidaDespesaMensalModel model = new LiquidaDespesaMensalModel();

                model.IdDespesaMensal = despesa.Id;
                model.DescricaoNaturezaDespesaMensal = despesa.NaturezaContaDespesa.DescricaoNaturezaConta;
                model.DescricaoTipoFormaLiquidacao = UtilEnum.GetTextoFormaLiquidacao(despesa.CodigoTipoFormaLiquidacao);
                model.TextoDescricaoDespesaMensal = despesa.DescricaoDespesa;
                model.DataVencimentoDespesa = despesa.DataVencimentoDespesa.ToString("dd/MM/yyyy");
                model.ValorDespesa = despesa.ValorDespesa;
                model.DataLiquidacaoDespesa = DateTime.Now.Date;
                model.ValorTotalLiquidacaoDespesa = despesa.ValorDespesa.ToString("N");

                return JsonResultSucesso(RenderRazorViewToString("_LiquidarDespesaMensalPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonLiquidarDespesaMensal(idDespesa)"));
            }

        }

        // POST /DespesaMensal/JsonLiquidarDespesaMensal
        [HttpPost]
        public JsonResult JsonLiquidarDespesaMensal(LiquidaDespesaMensalModel model)
        {

            if (!ModelState.IsValid)
                return JsonResultErro(ModelState.Errors());

            try
            {
                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                negocio.LiquidarDespesa(model.IdDespesaMensal, model.DataLiquidacaoDespesa, model.ValorDescontoLiquidacaoDespesa, model.ValorMultaJurosLiquidacaoDespesa, model.TextoObservacaoLiquidacaoDespesa);
                return JsonResultSucesso("Sua despesa foi liquidada com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonLiquidarReceitaMensal(LiquidaReceitaMensalModel)"));
            }
        }

        //
        // GET /DespesaMensal/JsonListarDespesaMensalSimples
        [HttpGet]
        public JsonResult JsonListarDespesaMensalSimples()
        {
            try
            {
                return JsonResultSucesso(RenderRazorViewToString("_ListaDespesaSimplesPartial", ListarDespesasDoMes()));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarDespesaMensalSimples()"));
            }
        }

        /// <summary>
        /// Recupera lista de totais de despesas vencidas nos meses anteriores ao corrente;
        /// </summary>
        /// <returns></returns>
        private List<DespesaMensal> ListarDespesasVencidasMesesAnteriores()
        {
            Negocio.DespesaNegocio negocio = new Negocio.DespesaNegocio(UsuarioLogadoConfig.Instance);
            return negocio.ListarTotaisDespesasVencidasMesesAnteriores(DateTime.Now.Month, DateTime.Now.Year);
        }

        /// <summary>
        /// Recupera a lista de despesas do mês corrente, somente as em aberto.
        /// </summary>
        /// <returns></returns>
        private List<Dados.Models.DespesaMensal> ListarDespesasDoMes()
        {
            Negocio.DespesaNegocio negocio = new Negocio.DespesaNegocio(UsuarioLogadoConfig.Instance);
            Negocio.Filtros.FiltroDespesaMensal filtroDespesas = new Negocio.Filtros.FiltroDespesaMensal();
            filtroDespesas.DataInicialVencimento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            filtroDespesas.DataFinalVencimento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Negocio.UtilNegocio.ObterDiasMes(DateTime.Now.Month, DateTime.Now.Year), 23, 59, 59);
            filtroDespesas.HasAbertas = true;
            filtroDespesas.HasVencidas = true;
            return negocio.ListarDespesaMensal(filtroDespesas);
        }

        //
        // GET: /DespesaMensal/JsonRegistrarNovaDespesa
        public JsonResult JsonRegistrarNovaDespesa()
        {
            try
            {
                DespesaMensalModel model = new DespesaMensalModel();
                model.DataVencimentoDespesa = DateTime.Now.Date;
                model.DropboxFormaLiquidacao = DropboxHelper.DropboxFormaLiquidacao();
                model.DropboxNaturezaDespesa = DropboxHelper.DropboxNaturezaDespesa();
                return JsonResultSucesso(RenderRazorViewToString("_RegistrarDespesaSimplesPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRegistrarNovaDespesa()"));
            }
        }

        //
        // POST: /DespesaMensal/JsonRegistrarNovaDespesa
        [HttpPost]
        public JsonResult JsonRegistrarNovaDespesa(DespesaMensalModel model)
        {

            if (!ModelState.IsValid)
                return JsonResultErro(ModelState.Errors());

            try
            {

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                string strLiquidacao = string.Empty;

                var despesaMensal = new DespesaMensal();
                despesaMensal.IdNaturezaContaDespesa = model.IdNaturezaDespesaMensal;
                despesaMensal.CodigoTipoFormaLiquidacao = model.CodigoFormaPagamentoDespesaMensal;
                despesaMensal.CodigoVinculoFormaLiquidacao = (model.IdVinculoFormaLiquidacao.HasValue ? model.IdVinculoFormaLiquidacao.Value : 0);
                despesaMensal.IsDespesaParcelada = model.IsDespesaMensalParcelada;
                despesaMensal.DescricaoDespesa = model.TextoDescricaoDespesaMensal;
                despesaMensal.DataVencimentoDespesa = model.DataVencimentoDespesa;
                despesaMensal.ValorDespesa = model.ValorDespesa;
                if (model.IsDespesaLiquidada)
                {//Registrar uma despesa já liquidada.
                    despesaMensal.IsDespesaLiquidada = model.IsDespesaLiquidada;
                    despesaMensal.DataHoraLiquidacaoDespesa = (model.DataLiquidacaoDespesa.HasValue ? model.DataLiquidacaoDespesa.Value : model.DataVencimentoDespesa);
                    despesaMensal.ValorDescontoLiquidacaoDespesa = model.ValorDescontoLiquidacaoDespesa;
                    despesaMensal.ValorMultaJurosLiquidacaoDespesa = model.ValorMultaJurosLiquidacaoDespesa;
                    despesaMensal.ValorTotalLiquidacaoDespesa = negocio.CalcularTotalDespesaLiquidada(despesaMensal.ValorDespesa, despesaMensal.ValorDescontoLiquidacaoDespesa, despesaMensal.ValorMultaJurosLiquidacaoDespesa);
                    despesaMensal.TextoObservacaoDespesa = model.TextoObservacaoLiquidacaoDespesa;
                    strLiquidacao = " (já liquidada)";
                }

                negocio.RegistrarDespesa(despesaMensal);

                return JsonResultSucesso(string.Format("Registro de despesa mensal{0} efetuado com sucesso.", strLiquidacao));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRegistrarNovaDespesa(model)"));
            }
        }
        
        //
        // GET /DespesaMensal/JsonListarDespesaMensal
        [HttpGet]
        public JsonResult JsonListarDespesaMensal(string dataFiltro)
        {
            try
            {

                FiltroDespesaModel model = new FiltroDespesaModel();
                model.IsFiltroTodas = true;
                if (string.IsNullOrEmpty(dataFiltro))
                {
                    model.DataInicialFiltro = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                }
                else
                {
                    DateTime dataFiltroDespesa = DateTime.Parse(dataFiltro);
                    model.DataInicialFiltro = dataFiltroDespesa;
                }
                model.DropboxFiltroMesAno = DropboxHelper.DropboxMesesDoAno(model.DataInicialFiltro.Year);
                SetValueDropboxFiltroMesAno(model.DataInicialFiltro, model.DropboxFiltroMesAno);
                model.DespesasMensais = ListarDespesas(model);
                ObterTotaisDespesa(model);

                return JsonResultSucesso(RenderRazorViewToString("_ListaDespesaPartial", model));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonListarDespesaMensal(dataFiltro::{0})", dataFiltro)));
            }
        }

        //
        // GET /DespesaMensal/JsonConfirmarRemocao
        [HttpGet]
        public JsonResult JsonConfirmarRemocao(int idDespesa)
        {
            try
            {

                if (idDespesa == 0)
                    return JsonResultErro("Identificador da despesa mensal não informado.");

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                DespesaMensal despesa = negocio.ObterDespesaMensal(idDespesa);
                if (despesa == null)
                    return JsonResultErro(string.Format("Despesa mensal com o Id [{0}] não encontrada.", idDespesa));

                DespesaMensalConfirmaRemocaoModel model = new DespesaMensalConfirmaRemocaoModel();

                model.IdDespesaMensal = despesa.Id;
                model.DescricaoNaturezaDespesaMensal = despesa.NaturezaContaDespesa.DescricaoNaturezaConta;
                model.DescricaoTipoFormaLiquidacao = UtilEnum.GetTextoFormaLiquidacao(despesa.CodigoTipoFormaLiquidacao);
                model.TextoDescricaoDespesaMensal = despesa.DescricaoDespesa;
                model.DataVencimentoDespesa = despesa.DataVencimentoDespesa.ToString("dd/MM/yyyy");
                model.ValorDespesa = despesa.ValorDespesa.ToString("N");
                model.HasDespesaParcelada = despesa.IsDespesaParcelada;
                if (model.HasDespesaParcelada)
                    model.QtdParcelas = negocio.ObterQtdParcelasDespesa(despesa.CodigoDespesaParcelada.Value);

                return JsonResultSucesso(RenderRazorViewToString("_ConfirmarRemocaoDespesaMensalPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonConfirmarRemocao(idDespesa)"));
            }
        }

        //
        // POST /DespesaMensal/JsonRemoverDespesaMensal
        [HttpPost]
        public JsonResult JsonRemoverDespesaMensal(DespesaMensalConfirmaRemocaoModel model)
        {
            if (!ModelState.IsValid)
                return JsonResultErro(ModelState.Errors());

            try
            {

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverDespesaMensal(model.IdDespesaMensal, model.HasRemoverParcelamento);

                string mensagemResult = "Sua despesa foi removida com sucesso.";
                if (model.HasRemoverParcelamento)
                    mensagemResult = "Sua despesa foi removida, assim como todas as parcelas vinculadas.";
                return JsonResultSucesso(mensagemResult);

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRemoverDespesa(idDespesa)"));
            }
        }

        //
        // GET /DespesaMensal/JsonListarParcelamentoDespesa
        [HttpGet]
        public JsonResult JsonListarParcelamentoDespesa(int idDespesa)
        {

            try
            {

                if (idDespesa == 0)
                    return JsonResultErro("Identificador da despesa mensal não informado.");

                DespesaNegocio negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                DespesaMensal despesa = negocio.ObterDespesaMensal(idDespesa);
                if (despesa == null)
                    return JsonResultErro(string.Format("Despesa mensal com o Id [{0}] não encontrada.", idDespesa));

                if (!despesa.IsDespesaParcelada)
                    return JsonResultErro(string.Format("Despesa mensal com o Id [{0}] não é vinculada a nenhum parcelamento.", idDespesa));

                DespesaMensalParcelamentoListaModel model = new DespesaMensalParcelamentoListaModel();

                model.IdDespesaMensal = despesa.Id;
                model.DescricaoTipoFormaLiquidacao = UtilEnum.GetTextoFormaLiquidacao(despesa.CodigoTipoFormaLiquidacao);
                short qtdParcelas = (short)negocio.ObterQtdParcelasDespesa(despesa.CodigoDespesaParcelada.Value);
                model.QtdParcelasDespesa = qtdParcelas;

                //Recuperar as despesas vinculadas ao parcelamento...
                var listaParcelamento = negocio.ListarDespesasParcelamento(despesa.CodigoDespesaParcelada.Value);
                List<DespesaMensalParcelamentoListaParcelaModel> listaParcelasModel = new List<DespesaMensalParcelamentoListaParcelaModel>();
                foreach (var item in listaParcelamento)
                {
                    DespesaMensalParcelamentoListaParcelaModel parcelaModel = new DespesaMensalParcelamentoListaParcelaModel();
                    parcelaModel.DescricaoDespesaMensal = item.DescricaoDespesa;
                    int idxTraco = item.DescricaoDespesa.IndexOf("-");
                    if (idxTraco > 1)
                    {
                        string numeroParcelaDespesa = item.DescricaoDespesa.Substring(0, idxTraco);
                        parcelaModel.DescricaoDespesaMensal = item.DescricaoDespesa.Substring(idxTraco + 1);
                        parcelaModel.NumeroParcelaDespesa = numeroParcelaDespesa;
                    }

                    parcelaModel.DescricaoNaturezaDespesaMensal = item.NaturezaContaDespesa.DescricaoNaturezaConta;
                    parcelaModel.DataVencimentoDespesa = item.DataVencimentoDespesa;
                    parcelaModel.ValorDespesa = item.ValorDespesa;
                    if (item.IsDespesaLiquidada) {
                        parcelaModel.IsDespesaLiquidada = true;
                        parcelaModel.DataLiquidacaoDespesa = item.DataHoraLiquidacaoDespesa;
                    }
                    listaParcelasModel.Add(parcelaModel);
                }
                model.ParcelasDespesa = listaParcelasModel;

                return JsonResultSucesso(RenderRazorViewToString("_ListaParcelamentoDespesaPartial", model));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarParcelamentoDespesa(idDespesa)"));
            }

        }
    }
}
