using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class ContaCorrenteController : GenericController
    {
        //
        // GET: /ContaCorrente/

        public ActionResult Index()
        {
            ContaCorrenteModel model = new ContaCorrenteModel();
            return View(model);
        }

        //
        // POST: /ContaCorrente/Registrar
        [HttpPost]
        public ActionResult Registrar(ContaCorrenteModel model)
        {
            
            try
            {
                
                var contaCorrente = new ContaCorrente();
                contaCorrente.NumeroBanco = model.NumeroBanco;
                contaCorrente.NomeBanco = model.NomeBanco;
                contaCorrente.NumeroAgencia = model.NumeroAgencia;
                contaCorrente.NumeroContaCorrente = model.NumeroContaCorrente;
                contaCorrente.NomeTitularConta = model.NomeTitular;
                contaCorrente.ValorLimiteConta = model.ValorLimite;
                contaCorrente.IsContaCorrenteAtiva = model.IsContaCorrenteAtiva;

                var negocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
                negocio.RegistrarContaCorrente(contaCorrente);
                
                AlertaUsuario("Registro da conta corrente efetuado com sucesso.", TipoAlertaEnum.Informacao);
                
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Conta Corrente");
                return View("Index", model);
            }


        }

        //
        // GET: /ContaCorrente/Editar/{id}
        public ActionResult Editar(int id)
        {
            ContaCorrenteModel model = new ContaCorrenteModel();

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id da Conta Corrente não informado.");
                return View("Index", model);
            }

            var negocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
            ContaCorrente contaCorrente = negocio.ObterContaCorrente(id);
            if (contaCorrente==null || contaCorrente.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Conta Corrente com Id [{0}] não encontrada.", id));
                return View("Index", model);
            }

            model.IdContaCorrente = contaCorrente.Id;
            model.NumeroBanco = contaCorrente.NumeroBanco;
            model.NomeBanco = contaCorrente.NomeBanco;
            model.NumeroAgencia = contaCorrente.NumeroAgencia;
            model.NumeroContaCorrente = contaCorrente.NumeroContaCorrente;
            model.NomeTitular = contaCorrente.NomeTitularConta;
            model.ValorLimite = contaCorrente.ValorLimiteConta;
            model.IsContaCorrenteAtiva = contaCorrente.IsContaCorrenteAtiva;

            return View(model);

        }

        //
        // POST: /ContaCorrente/Editar/
        [HttpPost]
        public ActionResult Editar(ContaCorrenteModel model)
        {
            
            try
            {
                var negocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
                ContaCorrente contaCorrente = negocio.ObterContaCorrente(model.IdContaCorrente);
                if (contaCorrente == null || contaCorrente.Id == 0)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Conta Corrente com Id [{0}] não encontrada.", model.IdContaCorrente));
                    return View("Index", model);
                }

                contaCorrente.NumeroBanco = model.NumeroBanco;
                contaCorrente.NomeBanco = model.NomeBanco;
                contaCorrente.NumeroAgencia = model.NumeroAgencia;
                contaCorrente.NumeroContaCorrente = model.NumeroContaCorrente;
                contaCorrente.NomeTitularConta = model.NomeTitular;
                contaCorrente.ValorLimiteConta = model.ValorLimite;
                contaCorrente.IsContaCorrenteAtiva = model.IsContaCorrenteAtiva;

                negocio.GravarContaCorrente(contaCorrente);
                
                AlertaUsuario("Conta corrente alterada com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Editar Conta Corrente");
                return View("Editar", model);
            }

        }

        //
        // GET: /ContaCorrente/Remover/{id}

        public ActionResult Remover(int id)
        {
            var model = new ContaCorrenteModel();

            try
            {
                
                if (id == 0)
                {
                    ModelState.AddModelError(string.Empty, "Id da Conta Corrente não informado.");
                    return View("Index", model);
                }

                var negocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);

                negocio.RemoverContaCorrente(id);

                AlertaUsuario("Conta corrente removida com sucesso.", TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remoção Conta Corrente");
                return View("Index", model);
            }

        }

        [HttpGet]
        public JsonResult JsonListarContaCorrente()
        {
            try
            {
                var negocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
                var listaContaCorrente = negocio.ListarContaCorrente(false);
                return JsonResultSucesso(RenderRazorViewToString("_ListaContaCorrentePartial", listaContaCorrente));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarContaCorrente()"));
            }
        }

        [HttpPost]
        public JsonResult JsonRemoverContaCorrente(int idContaCorrente)
        {
            try
            {
                var negocio = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverContaCorrente(idContaCorrente);
                return JsonResultSucesso("Conta Corrente removida com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonRemoverContaCorrente(IdContaCorrente::{0})", idContaCorrente)));
            }
        }
    }
}
