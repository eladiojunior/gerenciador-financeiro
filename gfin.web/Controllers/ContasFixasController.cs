using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class ContasFixasController : GenericController
    {
        //
        // GET: /ContasFixas/Index
        [HttpGet]
        public ActionResult Index()
        {
            HistoricoVerificacaoContasFixasModel model = new HistoricoVerificacaoContasFixasModel();

            GFin.Negocio.ProcessoAutomaticoNegocio processoAutomaticoNegocio = new GFin.Negocio.ProcessoAutomaticoNegocio(UsuarioLogadoConfig.Instance);
            //Recuperar os 10 últimos processo de verificação de contas fixas...
            model.ListaUltimosProcessos = processoAutomaticoNegocio.ListarProcessos(Dados.Enums.TipoProcessoAutomaticoEnum.ContasFixasMensal, 10);
            return View(model);
        }

        //
        // GET: /ContasFixas/Processar
        [HttpGet]
        public ActionResult Processar()
        {
            if (!TimerVerificarContasFixas.Instancia().IsVerificando)
            {
                TimerVerificarContasFixas.Instancia().VerificarContasFixasDoMesPorEntidade(UsuarioLogadoConfig.Instance.UsuarioLogado.IdEntidade);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult JsonHistoricoProcesso(int idProcesso)
        {
            try
            {
                var negocio = new ProcessoAutomaticoNegocio(UsuarioLogadoConfig.Instance);

                ProcessoAutomatico processo = negocio.ObterProcesso(idProcesso);
                if (processo == null)
                    return JsonResultErro(string.Format("Não encontramos o processo com o ID [{0}].", idProcesso));

                HistoricoProcessoModel model = new HistoricoProcessoModel();
                model.DataHoraProcesso = processo.DataHoraRegistroProcesso.ToString("dd/MM/yyyy HH:mm:ss");
                model.NomeProcesso = processo.NomeProcessoAutomatico;
                model.ListaHistoricoProcesso = new List<HistoricoSituacaoProcesso>();

                //Historico do processo...
                var listaHistorico = negocio.ListarHistoricoSituacaoProcesso(processo.Id);
                if (listaHistorico!=null)
                {
                    model.ListaHistoricoProcesso = listaHistorico;
                }
                return JsonResultSucesso(RenderRazorViewToString("_ListaHistoricoProcessoPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonHistoricoProcesso(IdProcesso::{0})", idProcesso)));
            }
        }
    }
}
