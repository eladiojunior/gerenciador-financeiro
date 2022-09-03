using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace GFin.Web
{
    public class MvcApplication : HttpApplication
    {
        private const string KeyAppLogger = "GFin";

        protected void Application_Start()
        {
            
            AppLogger.Log.InitLog(KeyAppLogger, new AppLoggerConfig(), new AppLoggerUsuario());

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Agendar o timer para verificar as despesas e receitas fixas...
            TimerVerificarContasFixas.Instancia();

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var pag = context.Request.RawUrl;
            var erro = Context.Server.GetLastError().GetBaseException();
            var pathErro = System.Configuration.ConfigurationManager.AppSettings["urlPaginaErro"];
            var logEntry = AppLogger.Log.Get().LogError(erro, pag);
            Response.Redirect(
                $"{pathErro}?Id={logEntry.Id}&dataHora={logEntry.DataHora:s}&app={KeyAppLogger}");
        }

    }
}