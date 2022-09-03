using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using GFin.Dados.Enums;

namespace GFin.Web
{
    public class UsuarioFilterAttribute : System.Web.Mvc.AuthorizeAttribute
    {

        public UsuarioFilterAttribute(params TipoPerfilAcessoUsuarioEnum[] perfils)
        {
            string roles = "";
            foreach (var item in perfils)
                roles += (short)item + ";";
            this.Roles = roles;
        }

        /// <summary>
        /// Método de autenticação que verifica se o usuário está autenticado e se sua informações 
        /// tem permissão para acessar o que está sendo protegido.
        /// </summary>
        /// <param name="filterContext">HTTP Contexto.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (IsAllowAnonymous(filterContext)) return;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    JsonResult result = new JsonResult();
                    result.Data = new { HasErro = true, Erros = "Usuário não autenticado." };
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    filterContext.Result = result;
                    return;
                }
            }
            else
            {
                if (!UsuarioLogadoConfig.Instance.HasUsuarioLogado())
                {//Nenhum usuário logado, solicitar autenticação do usuário...
                    UsuarioLogadoConfig.Instance.Logoff();
                    string urlReturn = ObterUrlReturn(filterContext.HttpContext.Request);
                    System.Web.Helpers.WebCache.Set("returnUrl", urlReturn, 1);
                    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
                    return;
                }
                var _usuarioLogado = UsuarioLogadoConfig.Instance.UsuarioLogado;
                if ((!String.IsNullOrEmpty(Users) && !Users.Contains(_usuarioLogado.EmailUsuario)) ||
                    //Verificar se o usuário pode acessar o sistema, quando informado.
                    (!String.IsNullOrEmpty(Roles) && !UsuarioLogadoConfig.Instance.IsPerfil(Roles)))
                {
                    filterContext.Result = RedirectAcessoNegado();
                    return;
                }
            }

        }

        /// <summary>
        /// Retorna a URL de Login adicionando a página de retorno caso exista, e seja diferente de Login.
        /// </summary>
        /// <param name="request">Request para recuperação do path de retorno.</param>
        /// <returns></returns>
        private string ObterUrlReturn(HttpRequestBase request)
        {
            string urlReturn = request.Path;
            string urlLogin = FormsAuthentication.LoginUrl;
            if (string.IsNullOrEmpty(urlReturn) || 
                urlLogin.Equals(urlReturn, StringComparison.CurrentCultureIgnoreCase))
                return "/";
            return urlReturn;
        }

        /// <summary>
        /// Gera RedirectResult para a página de acesso negado.
        /// </summary>
        /// <returns></returns>
        private RedirectResult RedirectAcessoNegado()
        {
            var urlErroAcessoNegado = WebConfigurationManager.AppSettings["urlErroAcessoNegado"];
            return new RedirectResult(urlErroAcessoNegado);
        }

        /// <summary>
        /// Verificar se foi solicita a proteção da controller ou Action.
        /// Se não foi, [AllowAnonymous], então não verificar acesso.
        /// </summary>
        /// <param name="filterContext">HTTP Contexto.</param>
        /// <returns></returns>
        private bool IsAllowAnonymous(AuthorizationContext filterContext)
        {
            bool allowAnonymous =
                filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            return allowAnonymous;
        }

    }
}