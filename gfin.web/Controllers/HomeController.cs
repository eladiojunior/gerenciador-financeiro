using GFin.Core.Negocio.DTOs;
using GFin.Negocio;
using GFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class HomeController : GenericController
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /FaleConosco/
        [AllowAnonymous]
        public ActionResult FaleConosco()
        {
            return View();
        }

        [HttpGet]

        public JsonResult JsonGraficoContasAnual()
        {
            int anoCorrente = DateTime.Now.Year;
            var negocioDespesa = new DespesaNegocio(UsuarioLogadoConfig.Instance);
            var negocioReceita = new DespesaNegocio(UsuarioLogadoConfig.Instance);

            List<TotalDespesaMensalDTO> listaTotaisDespesa = negocioDespesa.ListarTotaisDespesasAnual(anoCorrente);
            List<TotalReceitaMensalDTO> listaTotaisReceita = negocioReceita.ListarTotaisReceitasAnual(anoCorrente);

            ChartModel _chart = new ChartModel();
            _chart.type = "line";
            _chart.data.labels = listaTotaisDespesa.Select(s => s.MesCompetencia.ToString("MM/yyyy")).ToArray();
            _chart.data.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "Despesas",
                data = listaTotaisDespesa.Select(s => s.ValorTotalMes.ToString().Replace(".", "").Replace(",", ".")).ToArray(),
                borderColor = new string[] { "#e60000" },
                backgroundColor = new string[] { "#e60000" },
                borderWidth = 2
            });
            _dataSet.Add(new Datasets()
            {
                label = "Receitas",
                data = listaTotaisReceita.Select(s => s.ValorTotalMes.ToString().Replace(".", "").Replace(",", ".")).ToArray(),
                borderColor = new string[] { "#0d73d9" },
                backgroundColor = new string[] { "#0d73d9" },
                borderWidth = 2
            });
            _chart.data.datasets = _dataSet;
            _chart.options = null;
            /*
            _chart.options = new Options()
            {
                responsive = true,
                maintainAspectRatio = false,
                title = new Title() { display = true, text = "Despesas e Receitas" }
            };
            */
            return JsonResultSucesso(_chart, "Gráfico carregado...");
        }
    }
}
