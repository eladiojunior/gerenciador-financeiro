using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
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
    public class ChequeController : GenericController
    {
        
        //
        // GET: /Cheque/
        public ActionResult Index()
        {
            ChequeRegistroModel model = new ChequeRegistroModel();
            model.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrente();
            model.FiltroCheque = new Models.Filtros.FiltroChequeModel();
            model.FiltroCheque.DropboxTipoSituacaoCheque = DropboxHelper.DropboxTipoSituacaoCheque();
            return View(model);
        }

        //
        // POST: /Cheque/Registrar/
        [HttpPost]
        public ActionResult Registrar(ChequeRegistroModel model)
        {
            List<Cheque> listaCheques = new List<Cheque>();
            try {
                foreach (var item in model.ChequesRegistro)
                {
                    if (item.NumeroCheque != 0 && item.CodigoSituacaoCheque == 0)
                    {
                        var cheque = new Cheque();
                        cheque.IdContaCorrente = model.IdBancoAgenciaContaCorrente;
                        cheque.CodigoSituacaoCheque = (short)TipoSituacaoChequeEnum.ChequeRegistrado;
                        cheque.NumeroCheque = item.NumeroCheque;
                        listaCheques.Add(cheque);
                    }
                }
                ChequeNegocio chequeNegocio = new ChequeNegocio(UsuarioLogadoConfig.Instance);
                chequeNegocio.RegistrarCheques(listaCheques);
                
                //Carregar informaçoes para view.
                model.NumeroChequeInicial = null;
                model.NumeroChequeFinal = null;
                model.DropboxBancoAgenciaContaCorrente = DropboxHelper.DropboxBancoAgenciaContaCorrente();
                model.FiltroCheque = new Models.Filtros.FiltroChequeModel();
                model.FiltroCheque.DropboxTipoSituacaoCheque = DropboxHelper.DropboxTipoSituacaoCheque();
                model.FiltroCheque.ListaCheques = chequeNegocio.ListarCheque(model.IdBancoAgenciaContaCorrente);

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Cheques.");
            }
            return View("Index", model);
        }

        //
        // GET: /Cheque/Cancelar/
        [HttpGet]
        public ActionResult Cancelar(int id)
        {
            try
            {
                
                ChequeCancelado chequeCancelado = new ChequeCancelado();
                chequeCancelado.IdCheque = id;
                chequeCancelado.IsCancelamentoBanco = false;
                chequeCancelado.DataCancelamentoCheque = DateTime.Now;
                chequeCancelado.ObservacaoCancelamentoCheque = "Cheque cancelado a pedido do usuário.";

                ChequeNegocio chequeNegocio = new ChequeNegocio(UsuarioLogadoConfig.Instance);
                chequeNegocio.CancelarCheque(chequeCancelado);

                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Cheques.");
                return View("Index");
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
                situacaoCheque.CodigoSituacaoCheque = cheque.CodigoSituacaoCheque;
                situacaoCheque.DescicaoSituacaoCheque = UtilEnum.GetTextoTipoSituacaoCheque(cheque.CodigoSituacaoCheque);
            }
            else
            {
                situacaoCheque.CodigoSituacaoCheque = 0; //Não Registrado
                situacaoCheque.DescicaoSituacaoCheque = "Número do cheque validado";
            }
            return situacaoCheque;
        }

        //
        // POST: /Cheque/ListarChequesRegistro
        [HttpPost]
        public JsonResult JsonRegistrarListarCheques(ChequeRegistroModel model)
        {

            int numeroChequeIncial = model.NumeroChequeInicial.HasValue ? model.NumeroChequeInicial.Value : 0;
            int numeroChequeFinal = model.NumeroChequeFinal.HasValue ? model.NumeroChequeFinal.Value : 0;

            if (numeroChequeIncial > numeroChequeFinal)
            {//Erro número inicial superior ao final.
                return JsonResultErro("Número incial do Cheque não pode ser maior que o número final.");
            }

            try
            {

                ContaCorrenteNegocio contaCorrenteNegocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
                ContaCorrente conta = contaCorrenteNegocio.ObterContaCorrente(model.IdBancoAgenciaContaCorrente);
                if (conta != null)
                {
                    int qtdCheques = 0;
                    model.ChequesRegistro = new List<SituacaoChequeModel>();
                    model.BancoAgenciaContaCorrente = conta.BancoAgenciaContaCorrente;
                    for (int numeroCheque = numeroChequeIncial; numeroCheque <= numeroChequeFinal; numeroCheque++)
                    {
                        qtdCheques++;
                        SituacaoChequeModel situacaoCheque = ObterSituacaoCheque(conta.Id, numeroCheque);
                        model.ChequesRegistro.Add(situacaoCheque);
                    }
                    model.QtdChequeRegistro = qtdCheques;
                }

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRegistrarListarCheques(ChequeRegistroModel)"));
            }

            return JsonResultSucesso(RenderRazorViewToString("_ListaChequeRegistroPartial", model));

        }

        [HttpGet]
        public JsonResult JsonListarCheque(int idBancoAgenciaContaCorrente, short codigoSituacaoCheque)
        {
            try
            {
                ChequeNegocio chequeNegocio = new ChequeNegocio(UsuarioLogadoConfig.Instance);
                FiltroChequeModel model = new FiltroChequeModel();
                model.CodigoSituacaoCheque = codigoSituacaoCheque;
                model.IdBancoAgenciaContaCorrente = idBancoAgenciaContaCorrente;
                model.DropboxTipoSituacaoCheque = DropboxHelper.DropboxTipoSituacaoCheque();
                model.ListaCheques = new List<Cheque>();
                if (codigoSituacaoCheque != 0)
                {
                    var tipoCheque = UtilEnum.GetTipoSituacaoCheque(codigoSituacaoCheque);
                    model.ListaCheques = chequeNegocio.ListarCheque(idBancoAgenciaContaCorrente, tipoCheque);
                }
                else
                {
                    model.ListaCheques = chequeNegocio.ListarCheque(idBancoAgenciaContaCorrente);
                }
                return JsonResultSucesso(RenderRazorViewToString("_ListaChequePartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonListarCheque(idBancoAgenciaContaCorrente::{0}, codigoSituacaoCheque::{1})", idBancoAgenciaContaCorrente, codigoSituacaoCheque)));
            }
        }

        [HttpPost]
        public JsonResult JsonCancelarCheque(int idCheque)
        {
            try
            {
                var negocio = new ChequeNegocio(UsuarioLogadoConfig.Instance);

                ChequeCancelado chequeCancelado = new ChequeCancelado();
                chequeCancelado.IdCheque = idCheque;
                chequeCancelado.IsCancelamentoBanco = false;
                chequeCancelado.DataCancelamentoCheque = DateTime.Now;
                chequeCancelado.ObservacaoCancelamentoCheque = "Cheque cancelado a pedido do usuário.";

                negocio.CancelarCheque(chequeCancelado);

                return JsonResultSucesso("Cheque cancelado com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonCancelarCheque(IdCheque::{0})", idCheque)));
            }
        }

        [HttpGet]
        public JsonResult JsonHistoricoCheque(int idCheque)
        {
            try
            {
                var negocio = new ChequeNegocio(UsuarioLogadoConfig.Instance);

                Cheque chequeRegistrado = negocio.ObterCheque(idCheque);
                if (chequeRegistrado == null)
                    return JsonResultErro(string.Format("Não encontramos o cheque com o ID [{0}].", idCheque));

                HistoricoChequeModel model = new HistoricoChequeModel();
                model.IdBancoAgenciaContaCorrente = chequeRegistrado.IdContaCorrente;
                model.BancoAgenciaContaCorrente = chequeRegistrado.ContaCorrente.BancoAgenciaContaCorrente;
                model.NumeroCheque = chequeRegistrado.NumeroCheque;
                model.DescricaoSituacaoAtualCheque = UtilEnum.GetTextoTipoSituacaoCheque(chequeRegistrado.CodigoSituacaoCheque);
                List<ChequeModel> listaHistoricoCheque = new List<ChequeModel>();
                //Historico de registro...
                ChequeModel chequeModel = new ChequeModel();
                chequeModel.CodigoSituacaoCheque = (int)Dados.Enums.TipoSituacaoChequeEnum.ChequeRegistrado;
                chequeModel.DescicaoSituacaoCheque = UtilEnum.GetTextoEnum(Dados.Enums.TipoSituacaoChequeEnum.ChequeRegistrado);
                chequeModel.DataHoraHistoricoCheque = chequeRegistrado.DataHoraRegistroCheque;
                chequeModel.HistoricoCheque = "Registro do cheque.";
                listaHistoricoCheque.Add(chequeModel);
                //Historico de emissão...
                List<ChequeEmitido> listaChequeEmitido = negocio.ListarChequeEmitido(idCheque);
                if (listaChequeEmitido != null && listaChequeEmitido.Any())
                {
                    foreach (var item in listaChequeEmitido)
                    {
                        ChequeModel cheque = new ChequeModel();
                        cheque.CodigoSituacaoCheque = (int)Dados.Enums.TipoSituacaoChequeEnum.ChequeEmitido;
                        cheque.DescicaoSituacaoCheque = UtilEnum.GetTextoEnum(Dados.Enums.TipoSituacaoChequeEnum.ChequeEmitido);
                        cheque.DataHoraHistoricoCheque = item.DataEmissaoCheque;
                        cheque.HistoricoCheque = string.Format("Valor Emissão: {0} - {1}",item.ValorChequeEmitido.ToString("N"), item.HistoricoEmissaoCheque);
                        listaHistoricoCheque.Add(cheque);
                    }
                }
                //Historico de compensado...
                List<ChequeCompensado> listaChequeCompensado = negocio.ListarChequeCompensado(idCheque);
                if (listaChequeCompensado != null && listaChequeCompensado.Any())
                {
                    foreach (var item in listaChequeCompensado)
                    {
                        ChequeModel cheque = new ChequeModel();
                        cheque.CodigoSituacaoCheque = (int)Dados.Enums.TipoSituacaoChequeEnum.ChequeCompensado;
                        cheque.DescicaoSituacaoCheque = UtilEnum.GetTextoEnum(Dados.Enums.TipoSituacaoChequeEnum.ChequeCompensado);
                        cheque.DataHoraHistoricoCheque = item.DataCompensacaoCheque;
                        cheque.HistoricoCheque = item.ObservacaoChequeCompensado;
                        listaHistoricoCheque.Add(cheque);
                    }
                }
                //Historico de devolvido...
                List<ChequeDevolvido> listaChequeDevolvido = negocio.ListarChequeDevolvido(idCheque);
                if (listaChequeDevolvido != null && listaChequeDevolvido.Any())
                {
                    foreach (var item in listaChequeDevolvido)
                    {
                        ChequeModel cheque = new ChequeModel();
                        cheque.CodigoSituacaoCheque = (int)Dados.Enums.TipoSituacaoChequeEnum.ChequeDevolvido;
                        cheque.DescicaoSituacaoCheque = UtilEnum.GetTextoEnum(Dados.Enums.TipoSituacaoChequeEnum.ChequeDevolvido);
                        cheque.DataHoraHistoricoCheque = item.DataDevolucaoCheque;
                        cheque.HistoricoCheque = item.ObservacaoDevolucaoCheque;
                        listaHistoricoCheque.Add(cheque);
                    }
                }
                //Historico de cancelado...
                List<ChequeCancelado> listaChequeCancelado = negocio.ListarChequeCancelado(idCheque);
                if (listaChequeCancelado != null && listaChequeCancelado.Any())
                {
                    foreach (var item in listaChequeCancelado)
                    {
                        ChequeModel cheque = new ChequeModel();
                        cheque.CodigoSituacaoCheque = (int)Dados.Enums.TipoSituacaoChequeEnum.ChequeCancelado;
                        cheque.DescicaoSituacaoCheque = UtilEnum.GetTextoEnum(Dados.Enums.TipoSituacaoChequeEnum.ChequeCancelado);
                        cheque.DataHoraHistoricoCheque = item.DataCancelamentoCheque;
                        cheque.HistoricoCheque = string.Format("Cancelado no Banco: {0} - {1}", item.IsCancelamentoBanco?"Sim":"Não", item.ObservacaoCancelamentoCheque);
                        listaHistoricoCheque.Add(cheque);
                    }
                }
                //Historico de resgatado...
                List<ChequeResgatado> listaChequeResgatado = negocio.ListarChequeResgatado(idCheque);
                if (listaChequeResgatado != null && listaChequeResgatado.Any())
                {
                    foreach (var item in listaChequeResgatado)
                    {
                        ChequeModel cheque = new ChequeModel();
                        cheque.CodigoSituacaoCheque = (int)Dados.Enums.TipoSituacaoChequeEnum.ChequeResgatado;
                        cheque.DescicaoSituacaoCheque = UtilEnum.GetTextoEnum(Dados.Enums.TipoSituacaoChequeEnum.ChequeResgatado);
                        cheque.DataHoraHistoricoCheque = item.DataResgateCheque;
                        cheque.HistoricoCheque = string.Format("Valor Resgate: {0}, Baixa no CCF: {1} - {2}", item.ValorResgateCheque.ToString("N"), item.IsBaixaChequeCCF? string.Format("Sim - Valor Baixa CCF: {0}", item.ValorBaixaChequeCCF.ToString("N")) : "Não", item.ObservacaoResgateCheque);
                        listaHistoricoCheque.Add(cheque);
                    }
                }
                model.ListaHistoricoCheque = listaHistoricoCheque.OrderByDescending(o => o.DataHoraHistoricoCheque).ToList();
                return JsonResultSucesso(RenderRazorViewToString("_ListaHistoricoChequePartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonHistoricoCheque(IdCheque::{0})", idCheque)));
            }
        }
    }
}
