using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Web.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class CartaoCreditoController : GenericController
    {
        //
        // GET: /CartaoCredito/

        public ActionResult Index()
        {
            CartaoCreditoModel model = new CartaoCreditoModel();
            CarregarModelDefault(model);
            return View(model);
        }

        private void CarregarModelDefault(CartaoCreditoModel model)
        {
            model.DropboxTipoSituacaoCartao = Helpers.DropboxHelper.DropboxTipoSituacaoCartaoCredito();
            model.DropboxBancoAgenciaContaCorrente = Helpers.DropboxHelper.DropboxBancoAgenciaContaCorrenteCartao();
        }

        //
        // POST: /CartaoCredito/Registrar
        [HttpPost]
        public ActionResult Registrar(CartaoCreditoModel model)
        {
            
            try
            {

                var cartao = new CartaoCredito();
                if (model.IdBancoAgenciaContaCorrente != 0)
                {
                    cartao.IdContaCorrente = model.IdBancoAgenciaContaCorrente;
                }
                cartao.NumeroCartaoCredito = model.NumeroCartao;
                cartao.NomeCartaoCredito = model.NomeCartao;
                cartao.DataValidadeCartaoCredito = ConversorModel.ConverterDataValidade(model.MesAnoValidadeCartao);
                cartao.HasCartaoCredito = model.HasCartaoCredito;
                cartao.HasCartaoDebito = model.HasCartaoDebito;
                cartao.HasCartaoPrePago = model.HasCartaoPrePago;
                cartao.ValorLimiteCartaoCredito = model.ValorLimiteCartao;
                cartao.DiaVencimentoCartaoCredito = model.DiaVencimentoCartaoCredito;
                cartao.NomeProprietarioCartaoCredito = model.NomeProprietarioCartao;
                cartao.SituacaoCartaoCredito = model.SituacaoCartao;

                var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
                negocio.RegistrarCartaoCredito(cartao);
                
                AlertaUsuario("Registro do cartão de crédito efetuado com sucesso.", TipoAlertaEnum.Informacao);
                
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Cartão de Crédito");
                CarregarModelDefault(model);
                return View("Index", model);
            }


        }

        //
        // GET: /CartaoCredito/Editar/{id}
        public ActionResult Editar(int id)
        {
            CartaoCreditoModel model = new CartaoCreditoModel();
            CarregarModelDefault(model);

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id da Cartão de Crédito não informado.");
                return View("Index", model);
            }

            var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
            CartaoCredito cartaoCredito = negocio.ObterCartaoCredito(id);
            if (cartaoCredito==null || cartaoCredito.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Cartão de Crédito com Id [{0}] não encontrado.", id));
                return View("Index", model);
            }

            model.IdCartaoCredito = cartaoCredito.Id;
            model.IdBancoAgenciaContaCorrente = (cartaoCredito.IdContaCorrente.HasValue ? cartaoCredito.IdContaCorrente.Value : 0);
            model.NumeroCartao = cartaoCredito.NumeroCartaoCredito;
            model.NomeCartao = cartaoCredito.NomeCartaoCredito;
            model.MesAnoValidadeCartao = cartaoCredito.DataValidadeCartaoCredito.ToString("MM/yyyy");
            model.HasCartaoCredito = cartaoCredito.HasCartaoCredito;
            model.HasCartaoDebito = cartaoCredito.HasCartaoDebito;
            model.HasCartaoPrePago = cartaoCredito.HasCartaoPrePago;
            model.ValorLimiteCartao = cartaoCredito.ValorLimiteCartaoCredito;
            model.DiaVencimentoCartaoCredito = cartaoCredito.DiaVencimentoCartaoCredito;
            model.NomeProprietarioCartao = cartaoCredito.NomeProprietarioCartaoCredito;
            model.SituacaoCartao = cartaoCredito.SituacaoCartaoCredito;

            return View(model);

        }

        //
        // POST: /CartaoCredito/Editar/
        [HttpPost]
        public ActionResult Editar(CartaoCreditoModel model)
        {
            
            try
            {
                var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
                CartaoCredito cartaoCredito = negocio.ObterCartaoCredito(model.IdCartaoCredito);
                if (cartaoCredito == null || cartaoCredito.Id == 0)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Cartão de Crédito com Id [{0}] não encontrado.", model.IdCartaoCredito));
                    return View("Index", model);
                }

                cartaoCredito.NumeroCartaoCredito = model.NumeroCartao;
                cartaoCredito.NomeCartaoCredito = model.NomeCartao;
                cartaoCredito.DataValidadeCartaoCredito = ConversorModel.ConverterDataValidade(model.MesAnoValidadeCartao);
                cartaoCredito.HasCartaoCredito = model.HasCartaoCredito;
                cartaoCredito.HasCartaoDebito = model.HasCartaoDebito;
                cartaoCredito.HasCartaoPrePago = model.HasCartaoPrePago;
                cartaoCredito.ValorLimiteCartaoCredito = model.ValorLimiteCartao;
                cartaoCredito.DiaVencimentoCartaoCredito = model.DiaVencimentoCartaoCredito;
                cartaoCredito.NomeProprietarioCartaoCredito = model.NomeProprietarioCartao;
                cartaoCredito.SituacaoCartaoCredito = model.SituacaoCartao;

                negocio.GravarCartaoCredito(cartaoCredito);
                
                AlertaUsuario("Cartão de Crédito alterado com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Editar Cartão de Crédito");
                CarregarModelDefault(model);
                return View("Editar", model);
            }

        }

        //
        // GET: /CartaoCredito/Remover/{id}

        public ActionResult Remover(int id)
        {
            var model = new CartaoCreditoModel();
            CarregarModelDefault(model);

            try
            {
                
                if (id == 0)
                {
                    ModelState.AddModelError(string.Empty, "Id da Cartão de Crédito não informado.");
                    return View("Index", model);
                }

                var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);

                negocio.RemoverCartaoCredito(id);

                AlertaUsuario("Cartão de Crédito removido com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remoção Cartão de Crédito");
                return View("Index", model);
            }

        }
        //
        // POST: /CartaoCredito/ListarCartaoContaCorrente
        [HttpPost]
        public ActionResult ListarCartaoContaCorrente(int idContaCorrente)
        {
            List<CartaoCredito> listCartoes = null;
            try
            {
                var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
                listCartoes = negocio.ListarCartaoCredito(idContaCorrente, Dados.Enums.TipoSituacaoCartaoCreditoEnum.Ativo);
            }
            catch (Exception erro)
            {
                string msg = string.Format("Desculpe, não foi possível listar os cartões da conta corrente [{0}] informada.", idContaCorrente);
                TratarErroNegocio(erro, msg);
                throw new Exception(msg);
            }

            return PartialView("_ListaBasicaCartaoCreditoPartial", listCartoes);
        }

        [HttpGet]
        public JsonResult JsonListarCartaoCredito()
        {
            try
            {
                var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
                var listaCartaoCredito = negocio.ListarCartaoCredito(Dados.Enums.TipoSituacaoCartaoCreditoEnum.NaoInformado);
                return JsonResultSucesso(RenderRazorViewToString("_ListaCartaoCreditoPartial", listaCartaoCredito));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarCartaoCredito()"));
            }
        }

        [HttpPost]
        public JsonResult JsonRemoverCartaoCredito(int idCartaoCredito)
        {
            try
            {
                var negocio = new CartaoCreditoNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverCartaoCredito(idCartaoCredito);
                return JsonResultSucesso("Conta Corrente removida com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonRemoverCartaoCredito(IdCartaoCredito::{0})", idCartaoCredito)));
            }
        }
    }
}
