using GFin.Negocio;
using GFin.Dados.Models;
using GFin.Web.Controllers.Helpers;
using GFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class CorrentistaController : GenericController
    {
        public ActionResult Index()
        {
            CorrentistaModel model = new CorrentistaModel();
            CarregarModelDefault(model);
            return View(model);
        }

        private void CarregarModelDefault(CorrentistaModel model)
        {
            model.DropboxNomeBanco = DropboxHelper.DropboxNomeBancoCorrentista();
        }

        [HttpPost]
        public ActionResult Registrar(CorrentistaModel model)
        {
            try
            {

                Correntista correntista = new Correntista();
                correntista.NomeBanco = model.NomeBanco;
                correntista.NumeroAgencia = model.NumeroAgencia;
                correntista.NumeroContaCorrente = model.NumeroContaCorrente;
                correntista.NomeCorrentista = model.NomeCorrentista;
                correntista.Observacao = model.Observacao;

                CorrentistaNegocio correntistaNegocio = new CorrentistaNegocio(UsuarioLogadoConfig.Instance);
                correntistaNegocio.RegistrarCorrentista(correntista);
                
                AlertaUsuario("Registro do correntista na agenda efetuado com sucesso.", TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Correntista.");
                CarregarModelDefault(model);
                return View("Index", model);
            }
        }

        //
        // GET: /Correntista/Editar/{id}
        public ActionResult Editar(int id)
        {
            CorrentistaModel model = new CorrentistaModel();
            CarregarModelDefault(model);

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id do Correntista não informado.");
                return View("Index", model);
            }

            var negocio = new CorrentistaNegocio(UsuarioLogadoConfig.Instance);
            Correntista correntista = negocio.ObterCorrentista(id);
            if (correntista == null || correntista.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Correntista com Id [{0}] não encontrada.", id));
                return View("Index", model);
            }

            model.IdCorrentista = correntista.Id;
            model.NomeBanco = correntista.NomeBanco;
            model.NumeroAgencia = correntista.NumeroAgencia;
            model.NumeroContaCorrente = correntista.NumeroContaCorrente;
            model.NomeCorrentista = correntista.NomeCorrentista;
            model.Observacao = correntista.Observacao;

            return View(model);

        }

        //
        // POST: /Correntista/Editar/
        [HttpPost]
        public ActionResult Editar(CorrentistaModel model)
        {

            try
            {
                var negocio = new CorrentistaNegocio(UsuarioLogadoConfig.Instance);
                Correntista correntista = negocio.ObterCorrentista(model.IdCorrentista);
                if (correntista == null || correntista.Id == 0)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Correntista com Id [{0}] não encontrada.", model.IdCorrentista));
                    return View("Index", model);
                }

                correntista.NomeBanco = model.NomeBanco;
                correntista.NumeroAgencia = model.NumeroAgencia;
                correntista.NumeroContaCorrente = model.NumeroContaCorrente;
                correntista.NomeCorrentista = model.NomeCorrentista;
                correntista.Observacao = model.Observacao;

                negocio.GravarCorrentista(correntista);

                AlertaUsuario("Correntista alterado com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Editar Correntista");
                CarregarModelDefault(model);
                return View("Editar", model);
            }

        }

        //
        // GET: /Correntista/Remover/{id}

        public ActionResult Remover(int id)
        {
            var model = new CorrentistaModel();
            CarregarModelDefault(model);

            try
            {

                if (id == 0)
                {
                    ModelState.AddModelError(string.Empty, "Id do Correntista não informado.");
                    return View("Index", model);
                }

                var negocio = new CorrentistaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverCorrentista(id);

                AlertaUsuario("Correntista removido com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remoção Correntista");
                return View("Index", model);
            }

        }

        [HttpGet]
        public JsonResult JsonListarCorrentista()
        {
            try
            {
                var negocio = new CorrentistaNegocio(UsuarioLogadoConfig.Instance);
                var listaCorrentista = negocio.ListarCorrentistas();
                return JsonResultSucesso(RenderRazorViewToString("_ListaCorrentistaPartial", listaCorrentista));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarCorrentista()"));
            }
        }

        [HttpPost]
        public JsonResult JsonRemoverCorrentista(int idCorrentista)
        {
            try
            {
                var negocio = new CorrentistaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverCorrentista(idCorrentista);
                return JsonResultSucesso("Correntista removida com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonRemoverCorrentista(IdCorrentista::{0})", idCorrentista)));
            }
        }
    }
}