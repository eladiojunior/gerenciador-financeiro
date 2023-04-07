using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Web.Controllers.Helpers;
using GFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class ReceitaFixaController : GenericController
    {
        //
        // GET: /ReceitaFixa/

        public ActionResult Index()
        {
            ReceitaFixaModel model = new ReceitaFixaModel();
            CarregarModelDefault(model);
            return View(model);
        }

        private void CarregarModelDefault(ReceitaFixaModel model)
        {
            model.DropboxDiaRecebimento = DropboxHelper.DropboxDiasMes();
            model.DropboxNaturezaReceita = DropboxHelper.DropboxNaturezaReceita();
        }

        //
        // POST: /ReceitaFixa/Registrar
        [HttpPost]
        public ActionResult Registrar(ReceitaFixaModel model)
        {
            
            try
            {
                
                var receitaFixa = new ReceitaFixa();
                receitaFixa.IdNaturezaContaReceitaFixa = model.IdNaturezaContaReceitaFixa;
                receitaFixa.DiaRecebimentoReceitaFixa = model.NumeroDiaRecebimentoReceitaFixa;
                receitaFixa.DescricaoReceitaFixa = model.TextoDescricaoReceitaFixa;
                receitaFixa.ValorReceitaFixa = model.ValorReceitaFixa;
                receitaFixa.CodigoTipoSituacaoReceitaFixa = (short)TipoSituacaoEnum.Ativo;

                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RegistrarReceitaFixa(receitaFixa);
                
                AlertaUsuario("Registro de receita fixa efetuado com sucesso.", TipoAlertaEnum.Informacao);
                
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Receita Fixa");
                CarregarModelDefault(model);
                return View("Index", model);
            }


        }

        //
        // GET: /ReceitaFixa/Editar/{id}
        public ActionResult Editar(int id)
        {
            ReceitaFixaModel model = new ReceitaFixaModel();
            CarregarModelDefault(model);

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id da Receita Fixa não informado.");
                return View("Index", model);
            }

            var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
            ReceitaFixa receitaFixa = negocio.ObterReceitaFixa(id);
            if (receitaFixa==null || receitaFixa.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Receita Fixa com Id [{0}] não encontrada.", id));
                return View("Index", model);
            }

            model.IdReceitaFixa = receitaFixa.Id;
            model.IdNaturezaContaReceitaFixa = receitaFixa.IdNaturezaContaReceitaFixa;
            model.NumeroDiaRecebimentoReceitaFixa = receitaFixa.DiaRecebimentoReceitaFixa;
            model.TextoDescricaoReceitaFixa = receitaFixa.DescricaoReceitaFixa;
            model.ValorReceitaFixa = receitaFixa.ValorReceitaFixa;

            return View(model);

        }

        //
        // POST: /ReceitaFixa/Editar/
        [HttpPost]
        public ActionResult Editar(ReceitaFixaModel model)
        {
            
            try
            {
                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                ReceitaFixa receitaFixa = negocio.ObterReceitaFixa(model.IdReceitaFixa);
                if (receitaFixa != null && receitaFixa.Id != 0)
                {
                    receitaFixa.IdNaturezaContaReceitaFixa = model.IdNaturezaContaReceitaFixa;
                    receitaFixa.DiaRecebimentoReceitaFixa = model.NumeroDiaRecebimentoReceitaFixa;
                    receitaFixa.DescricaoReceitaFixa = model.TextoDescricaoReceitaFixa;
                    receitaFixa.ValorReceitaFixa = model.ValorReceitaFixa;
                    
                    negocio.GravarReceitaFixa(receitaFixa);
                }
                
                AlertaUsuario("Receita fixa alterada com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Editar Receita Fixa");
                CarregarModelDefault(model);
                return View("Editar", model);
            }

        }

        //
        // GET: /ReceitaFixa/Remover/{id}

        public ActionResult Remover(int id)
        {
            var model = new ReceitaFixaModel();
            CarregarModelDefault(model);

            try
            {
                
                if (id == 0)
                {
                    ModelState.AddModelError(string.Empty, "Id da Receita Fixa não informado.");
                    return View("Index", model);
                }

                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                ReceitaFixa receitaFixa = negocio.ObterReceitaFixa(id);
                if (receitaFixa == null || receitaFixa.Id == 0)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Receita Fixa com Id [{0}] não encontrada.", id));
                    return View("Index", model);
                }

                negocio.RemoverReceitaFixa(receitaFixa);

                AlertaUsuario("Receita fixa removida com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remoção Receita Fixa");
                return View("Index", model);
            }

        }

        [HttpGet]
        public JsonResult JsonListarReceitaFixa()
        {
            try
            {
                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                var listaReceitaFixa = negocio.ListarReceitaFixa(true);
                return JsonResultSucesso(RenderRazorViewToString("_ListaReceitaPartial", listaReceitaFixa));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarReceitaFixa()"));
            }
        }

        [HttpPost]
        public JsonResult JsonRemoverReceitaFixa(int idReceitaFixa)
        {
            try
            {
                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverReceitaFixa(idReceitaFixa);
                return JsonResultSucesso("Receita fixa removida com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonRemoverReceitaFixa(IdReceitaFixa::{0})", idReceitaFixa)));
            }
        }

    }
}
