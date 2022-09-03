using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Negocio.Listeners
{
    /// <summary>
    /// Interface que representa um usuário logado na camada de negócio.
    /// </summary>
    public interface IUsuarioLogado
    {
        bool IsLogado { get; }
        int IdUsuario { get; }
        string NomeUsuario { get; }
        string EmailUsuario { get; }
        DateTime DataUltimoAcessoUsuario { get; }
        int IdEntidade { get; }
        string NomeEntidade { get; }
        bool IsEmpresa { get; }
        string FotoBase64 { get; }
        short CodigoPerfil { get;}
        string NomePerfil { get; }

    }
}
