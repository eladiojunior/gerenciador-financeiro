using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Web.Controllers.Helpers;
using GFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class NaturezaContaController : GenericController
    {
        //
        // GET: /NaturezaConta/
        [HttpGet]
        public ActionResult Index()
        {
            NaturezaContaModel model = new NaturezaContaModel();
            CarregarModelDefault(model);
            return View(model);
        }

        private void CarregarModelDefault(NaturezaContaModel model)
        {
            model.DropboxTipoLancamentoConta = DropboxHelper.DropboxTipoLancamentoConta();
        }

        //
        // POST: /NaturezaConta/Registrar
        [HttpPost]
        public ActionResult Registrar(NaturezaContaModel model)
        {
            try
            {
                RegistrarNaturezaConta(model);
                
                AlertaUsuario("Registro da natureza da conta efetuado com sucesso.", TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Natureza da Conta");
                CarregarModelDefault(model);
                return View("Index", model);
            }
        }

        private NaturezaConta RegistrarNaturezaConta(NaturezaContaModel model)
        {
            
            var naturezaConta = new NaturezaConta();
            naturezaConta.CodigoTipoLancamentoConta = model.CodigoTipoLancamentoConta;
            naturezaConta.DescricaoNaturezaConta = model.DescricaoNaturezaConta;
            naturezaConta.CodigoTipoSituacaoNaturezaConta = (short)TipoSituacaoEnum.Ativo;

            var negocio = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
            negocio.RegistrarNatureza(naturezaConta);

            return naturezaConta;

        }

        //
        // GET: /NaturezaConta/Editar/{id}
        public ActionResult Editar(int id)
        {
            NaturezaContaModel model = new NaturezaContaModel();
            CarregarModelDefault(model);

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id da Natureza da Conta não informado.");
                return View("Index", model);
            }

            var negocio = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
            NaturezaConta naturezaConta = negocio.ObterNaturezaConta(id);
            if (naturezaConta==null || naturezaConta.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Natureza da Conta com Id [{0}] não encontrada.", id));
                return View("Index", model);
            }

            model.IdNaturezaConta = naturezaConta.Id;
            model.CodigoTipoLancamentoConta = naturezaConta.CodigoTipoSituacaoNaturezaConta;
            model.DescricaoNaturezaConta = naturezaConta.DescricaoNaturezaConta;
            
            return View(model);
        }

        //
        // POST: /NaturezaConta/Editar/
        [HttpPost]
        public ActionResult Editar(NaturezaContaModel model)
        {
            try
            {
                var negocio = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
                NaturezaConta naturezaConta = negocio.ObterNaturezaConta(model.IdNaturezaConta);
                if (naturezaConta != null && naturezaConta.Id != 0)
                {
                    naturezaConta.CodigoTipoSituacaoNaturezaConta = model.CodigoTipoLancamentoConta;
                    naturezaConta.DescricaoNaturezaConta = model.DescricaoNaturezaConta;
                    negocio.GravarNaturezaConta(naturezaConta);
                }
                
                AlertaUsuario("Natureza da conta alterada com sucesso.", TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Editar Natureza da Conta");
                CarregarModelDefault(model);
                return View("Editar", model);
            }
        }

        //
        // GET: /NaturezaConta/Remover/{id}

        public ActionResult Remover(int id)
        {

            NaturezaContaModel model = new NaturezaContaModel();
            CarregarModelDefault(model);

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id da Natureza da Conta não informado.");
                return View("Index", model);
            }

            try
            {

                var negocio = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverNaturezaConta(id);

                AlertaUsuario("Natureza da conta removida com sucesso.", TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remover Natureza da Conta");
                return View("Index", model);
            }

        }

        //
        // GET: /NaturezaConta/JsonNovaNaturezaDespesa
        [HttpGet]
        public JsonResult JsonNovaNaturezaDespesa(string retornoDropdown)
        {
            try
            {
                NovaNaturezaContaModel model = new NovaNaturezaContaModel();
                model.IdDropdownRetorno = retornoDropdown;
                model.CodigoTipoLancamentoConta = (int)GFin.Dados.Enums.TipoLancamentoEnum.Despesa;
                model.TipoLancamentoConta = GFin.Dados.Enums.UtilEnum.GetTextoEnum(GFin.Dados.Enums.TipoLancamentoEnum.Despesa);
                return JsonResultSucesso(RenderRazorViewToString("_NovaNaturezaPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonNovaNaturezaDespesa(retornoDropdown::{0})", retornoDropdown)));
            }
        }

        //
        // GET: /NaturezaConta/JsonNovaNaturezaReceita
        [HttpGet]
        public JsonResult JsonNovaNaturezaReceita(string retornoDropdown)
        {
            try
            {
                NovaNaturezaContaModel model = new NovaNaturezaContaModel();
                model.IdDropdownRetorno = retornoDropdown;
                model.CodigoTipoLancamentoConta = (int)GFin.Dados.Enums.TipoLancamentoEnum.Receita;
                model.TipoLancamentoConta = GFin.Dados.Enums.UtilEnum.GetTextoEnum(GFin.Dados.Enums.TipoLancamentoEnum.Receita);
                return JsonResultSucesso(RenderRazorViewToString("_NovaNaturezaPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonNovaNaturezaReceita(retornoDropdown::{0})", retornoDropdown)));
            }
        }

        //
        // POST: /NaturezaConta/JsonRegistrar
        [HttpPost]
        public JsonResult JsonRegistrar(NovaNaturezaContaModel model)
        {
            try
            {

                var naturezaConta = new NaturezaConta();
                naturezaConta.CodigoTipoLancamentoConta = model.CodigoTipoLancamentoConta;
                naturezaConta.DescricaoNaturezaConta = model.DescricaoNaturezaConta;
                naturezaConta.CodigoTipoSituacaoNaturezaConta = (short)TipoSituacaoEnum.Ativo;

                var negocio = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
                var natureza = negocio.RegistrarNatureza(naturezaConta);

                return JsonResultSucesso(natureza, "Nova natureza registrada com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRegistrar(NovaNaturezaContaModel)"));
            }
        }


        [HttpGet]
        public JsonResult JsonListarNaturezaConta()
        {
            try
            {
                var negocio = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
                var listaNaturezaConta = negocio.ListarNaturezas();
                return JsonResultSucesso(RenderRazorViewToString("_ListaNaturezaPartial", listaNaturezaConta));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarNaturezaConta()"));
            }
        }

        [HttpPost]
        public JsonResult JsonRemoverNaturezaConta(int idNaturezaConta)
        {
            try
            {
                var negocio = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverNaturezaConta(idNaturezaConta);
                return JsonResultSucesso("Natureza da conta removida com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonRemoverNaturezaConta(IdNaturezaConta::{0})", idNaturezaConta)));
            }
        }
    }

}
