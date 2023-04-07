using GFin.Negocio.Erros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public enum TipoAlertaEnum
    {
        Informacao=1, Atencao=2, Erro=9
    }

    [Authorize]
    public class GenericController : System.Web.Mvc.Controller
    {

        /// <summary>
        /// Cria um retorno Json de Erro (Result = false) com mensagem de erro.
        /// </summary>
        /// <param name="mensagemErro">Mensagem de erro que deve ser apresentada ao usuário.</param>
        /// <returns></returns>
        internal JsonResult JsonResultErro(string mensagemErro)
        {
            return Json(new { HasErro = true, Erros = new List<string>() { mensagemErro } }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cria um retorno Json de Erro (Result = false) com mensagem de erro.
        /// </summary>
        /// <param name="mensagensErro">Lista de mensagens de erro que deve ser apresentada ao usuário.</param>
        /// <returns></returns>
        internal JsonResult JsonResultErro(IEnumerable mensagensErro)
        {
            return Json(new { HasErro = true, Erros = mensagensErro }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cria um retorno Json de Sucesso (Result = true) com mensagem para o usuário (opcional).
        /// </summary>
        /// <param name="mensagemAlerta">Mensagem de alerta que deve ser apresentada ao usuário.</param>
        /// <returns></returns>
        internal JsonResult JsonResultSucesso(string mensagemAlerta = "")
        {
            return Json(new { HasErro = false, Mensagem = mensagemAlerta }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cria um retorno Json de Sucesso (Result = true) com Model e mensagem para o usuário (opcional).
        /// </summary>
        /// <param name="model">Informações do Model para renderizar a view.</param>
        /// <param name="mensagemAlerta">Mensagem de alerta que deve ser apresentada ao usuário.</param>
        /// <returns></returns>
        internal JsonResult JsonResultSucesso(object model, string mensagemAlerta = "")
        {
            return Json(new { HasErro = false, Model = model, Mensagem = mensagemAlerta }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Tratar as mensagens de erro de negócio.
        /// </summary>
        /// <param name="erro">Exception de erro para tratamento.</param>
        /// <param name="localErro">Local do erro, metodo.</param>
        internal void TratarErroNegocio(Exception erro, string localErro = null)
        {
            AppLogger.Log.Get().LogError(erro, localErro);
            if (erro.GetType() == typeof(NegocioException))
            {
                ModelState.AddModelError(string.Empty, (erro as NegocioException).Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, (localErro != null ? localErro + ": " : "") + "Desculpe, erro não esperado na sua solicitação.");
            }
        }

        /// <summary>
        /// Verifica se o erro é de negócio, retorna a mensagem do erro, caso contrário, mostra uma mensagem genérica.
        /// </summary>
        /// <param name="erro">Exception para verificação.</param>
        /// <param name="localErro">Local do erro, metodo.</param>
        /// <returns></returns>
        internal string TratarMensagemErroNegocio(Exception erro, string localErro = null)
        {
            AppLogger.Log.Get().LogError(erro, localErro);
            string mensagem = "Desculpe, erro não esperado na sua solicitação.";
            if (erro.GetType() == typeof(NegocioException))
            {
                mensagem = ((erro as NegocioException).Message);
            }
            return mensagem;
        }

        /// <summary>
        /// Envia uma mensagem de aleta (info, atenção ou erro) para o usuário.
        /// </summary>
        /// <param name="msg">Mensagem de alerta.</param>
        /// <param name="tipoAlerta">Tipo do alerta</param>
        internal void AlertaUsuario(string msg, TipoAlertaEnum tipoAlerta)
        {
            TempData["alertaMensagem"] = msg;
            TempData["alertaTipo"] = (Int16)tipoAlerta;
        }

        /// <summary>
        /// Renderizar a View em String para respostas Json;
        /// </summary>
        /// <param name="viewName">Nome da View a ser renderizada.</param>
        /// <param name="model">Informações da Model para carga.</param>
        /// <returns></returns>
        internal object RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}