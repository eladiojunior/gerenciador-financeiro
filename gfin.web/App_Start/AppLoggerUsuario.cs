using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web
{
    public class AppLoggerUsuario : AppLogger.Interfaces.IUserLog
    {
        public string GetLoginUsuario()
        {
            if (UsuarioLogadoConfig.Instance.UsuarioLogado.IsLogado)
                return UsuarioLogadoConfig.Instance.UsuarioLogado.EmailUsuario;
            return "Nenhum usuário";
        }

        public string GetNomeUsuario()
        {
            if (UsuarioLogadoConfig.Instance.UsuarioLogado.IsLogado)
                return UsuarioLogadoConfig.Instance.UsuarioLogado.NomeUsuario;
            return "Nenhum usuário";
        }

        public string GetPerfilUsuario()
        {
            if (UsuarioLogadoConfig.Instance.UsuarioLogado.IsLogado)
                return UsuarioLogadoConfig.Instance.UsuarioLogado.NomePerfil;
            return "Nenhum usuário";
        }
    }
}