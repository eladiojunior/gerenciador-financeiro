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
    public class DespesaFixaController : GenericController
    {
        //
        // GET: /DespesaFixa/

        public ActionResult Index()
        {
            DespesaFixaModel model = new DespesaFixaModel();
            CarregarModelDefault(model);
            return View(model);
        }

        private void CarregarModelDefault(DespesaFixaModel model)
        {
            model.DropboxDiaVencimento = DropboxHelper.DropboxDiasMes();
            model.DropboxNaturezaDespesa = DropboxHelper.DropboxNaturezaDespesa();
            model.DropboxFormaLiquidacao = DropboxHelper.DropboxFormaLiquidacao();
        }

        //
        // POST: /DespesaFixa/Registrar
        [HttpPost]
        public ActionResult Registrar(DespesaFixaModel model)
        {
            
            try
            {
                
                var despesaFixa = new DespesaFixa();
                despesaFixa.IdNaturezaContaDespesaFixa = model.IdNaturezaContaDespesaFixa;
                despesaFixa.DiaVencimentoDespesaFixa = model.NumeroDiaVencimentoDespesaFixa;
                despesaFixa.DescricaoDespesaFixa = model.TextoDescricaoDespesaFixa;
                despesaFixa.ValorDespesaFixa = model.ValorDespesaFixa;//.HasValue ? model.ValorDespesaFixa.Value : 0;
                despesaFixa.CodigoTipoSituacaoDespesaFixa = (short)TipoSituacaoEnum.Ativo;
                despesaFixa.CodigoTipoFormaLiquidacaoDespesaFixa = model.CodigoTipoFormaLiquidacao;

                var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RegistrarDespesaFixa(despesaFixa);
                
                AlertaUsuario("Registro de despesa fixa efetuado com sucesso.", TipoAlertaEnum.Informacao);
                
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Despesa Fixa");
                CarregarModelDefault(model);
                return View("Index", model);
            }


        }

        //
        // GET: /DespesaFixa/Editar/{id}
        public ActionResult Editar(int id)
        {
            DespesaFixaModel model = new DespesaFixaModel();
            CarregarModelDefault(model);

            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Id da Despesa Fixa não informado.");
                return View("Index", model);
            }

            var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
            DespesaFixa despesaFixa = negocio.ObterDespesaFixa(id);
            if (despesaFixa==null || despesaFixa.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Despesa Fixa com Id [{0}] não encontrada.", id));
                return View("Index", model);
            }

            model.IdDespesaFixa = despesaFixa.Id;
            model.IdNaturezaContaDespesaFixa = despesaFixa.IdNaturezaContaDespesaFixa;
            model.NumeroDiaVencimentoDespesaFixa = despesaFixa.DiaVencimentoDespesaFixa;
            model.TextoDescricaoDespesaFixa = despesaFixa.DescricaoDespesaFixa;
            model.ValorDespesaFixa = despesaFixa.ValorDespesaFixa;
            model.CodigoTipoFormaLiquidacao = despesaFixa.CodigoTipoFormaLiquidacaoDespesaFixa;

            return View(model);

        }

        //
        // POST: /DespesaFixa/Editar/
        [HttpPost]
        public ActionResult Editar(DespesaFixaModel model)
        {
            
            try
            {
                var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                DespesaFixa despesaFixa = negocio.ObterDespesaFixa(model.IdDespesaFixa);
                if (despesaFixa != null && despesaFixa.Id != 0)
                {
                    //Verificar se houve alteração do valor da despesa fixa...
                    bool hasGravarHistorico = (despesaFixa.ValorDespesaFixa != model.ValorDespesaFixa);

                    despesaFixa.IdNaturezaContaDespesaFixa = model.IdNaturezaContaDespesaFixa;
                    despesaFixa.DiaVencimentoDespesaFixa = model.NumeroDiaVencimentoDespesaFixa;
                    despesaFixa.DescricaoDespesaFixa = model.TextoDescricaoDespesaFixa;
                    despesaFixa.ValorDespesaFixa = model.ValorDespesaFixa;
                    despesaFixa.CodigoTipoFormaLiquidacaoDespesaFixa = model.CodigoTipoFormaLiquidacao;

                    negocio.GravarDespesaFixa(despesaFixa, hasGravarHistorico);
                }
                
                AlertaUsuario("Despesa fixa alterada com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Editar Despesa Fixa");
                CarregarModelDefault(model);
                return View("Editar", model);
            }

        }

        //
        // GET: /DespesaFixa/Remover/{id}

        public ActionResult Remover(int id)
        {
            var model = new DespesaFixaModel();
            CarregarModelDefault(model);

            try
            {
                
                if (id == 0)
                {
                    ModelState.AddModelError(string.Empty, "Id da Despesa Fixa não informado.");
                    return View("Index", model);
                }

                var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverDespesaFixa(id);

                AlertaUsuario("Despesa fixa removida com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remoção Despesa Fixa");
                return View("Index", model);
            }

        }

        [HttpGet]
        public JsonResult JsonListarDespesaFixa()
        {
            try
            {
                var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                var listaDespesaFixa = negocio.ListarDespesaFixa(true);
                return JsonResultSucesso(RenderRazorViewToString("_ListaDespesaPartial", listaDespesaFixa));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarDespesaFixa()"));
            }
        }

        [HttpPost]
        public JsonResult JsonRemoverDespesaFixa(int idDespesaFixa)
        {
            try
            {
                var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverDespesaFixa(idDespesaFixa);
                return JsonResultSucesso("Despesa fixa removida com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonRemoverDespesaFixa(IdDespesaFixa::{0})", idDespesaFixa)));
            }
        }
        
        //
        // POST /DespesaFixa/JsonListarHistoricoDespesaFixa
        [HttpPost]
        public JsonResult JsonListarHistoricoDespesaFixa(int idDespesaFixa)
        {
            try
            {
                HistoricoDespesaFixaModel model = new HistoricoDespesaFixaModel();

                var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
                var despesaFixa = negocio.ObterDespesaFixa(idDespesaFixa, true);
                if (despesaFixa == null)
                    return JsonResultErro(string.Format("Despesa Fixa com ID: {0} não encontrada.", idDespesaFixa));

                model.IdDespesaFixa = despesaFixa.Id;
                model.TextoDescricaoDespesaFixa = despesaFixa.DescricaoDespesaFixa;
                if (despesaFixa.ListaHistoricoDespesaFixa != null)
                {
                    model.HistoricoDespesasFixas = despesaFixa.ListaHistoricoDespesaFixa.OrderByDescending(o => o.DataHoraRegistroHistoricoDespesaFixa).ToList();
                }

                return JsonResultSucesso(RenderRazorViewToString("_HistoricoAtualizacaoPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonListarHistoricoDespesaFixa(IdDespesaFixa::{0})",idDespesaFixa)));
            }
        }

        //
        // GET /DespesaFixa/JsonGraficoHistoricoDespesaFixa
        [HttpGet]

        public JsonResult JsonGraficoHistoricoDespesaFixa(int idDespesaFixa)
        {
            var negocio = new DespesaNegocio(UsuarioLogadoConfig.Instance);
            var despesaFixa = negocio.ObterDespesaFixa(idDespesaFixa, true);
            if (despesaFixa == null)
                return JsonResultErro(string.Format("Despesa Fixa com ID: {0} não encontrada.", idDespesaFixa));
            var listaHistorico = despesaFixa.ListaHistoricoDespesaFixa;
            ChartModel _chart = new ChartModel();
            if (listaHistorico != null && listaHistorico.Any())
            {
                _chart.type = "line";
                _chart.data.labels = listaHistorico.Select(s => s.DataHoraRegistroHistoricoDespesaFixa.ToString("dd/MM/yyyy HH:mm:ss")).ToArray();
                _chart.data.datasets = new List<Datasets>();
                List<Datasets> _dataSet = new List<Datasets>();
                _dataSet.Add(new Datasets()
                {
                    label = "Evolução da Despesa Fixa",
                    data = listaHistorico.Select(s => s.ValorHistoricoDespesaFixa.ToString().Replace(".","").Replace(",",".")).ToArray(),
                    borderColor = new string[] { "#000000" },
                    borderWidth = 1
                });
                _chart.data.datasets = _dataSet;
                _chart.options = null;
            }
            return JsonResultSucesso(_chart, "Gráfico carregado...");
        }
    }
}
