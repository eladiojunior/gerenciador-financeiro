using gfin.webapi.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gfin.webapi.Dados
{
    internal class UsuarioAcessoEntidadeControleDAO : GenericDAO<UsuarioAcessoEntidadeControle>
    {
        internal UsuarioAcessoEntidadeControleDAO(GFinContext dbContexto) : base(dbContexto) { }

        /// <summary>
        /// Listar as entidades de controle financeiro pelo login (e-mail).
        /// </summary>
        /// <param name="login">Login de usuário.</param>
        /// <returns></returns>
        internal List<EntidadeControle> ListarPorLoginUsuario(string login)
        {
            return DbContextoGFin().UsuarioAcessoEntidadeControle.Where(uaec => uaec.UsuarioAcesso.EmailUsuario.Equals(login, StringComparison.CurrentCultureIgnoreCase) && 
                uaec.DataInicialVigenciaAcessoUsuario <= DateTime.Now && (uaec.DataFinallVigenciaAcessoUsuario == null || uaec.DataFinallVigenciaAcessoUsuario >= DateTime.Now))
                .Select(c => c.EntidadeControle).Distinct().ToList();
        }

        /// <summary>
        /// Recupera a entidade de controle, cuja o usuário está vinculado como responsável.
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário para recuperação de sua entidade de controle.</param>
        /// <returns></returns>
        internal EntidadeControle ObterEntidadeControleUsuario(int idUsuario)
        {
            return DbContextoGFin().UsuarioAcessoEntidadeControle.Where(uaec => uaec.IdUsuarioAcesso == idUsuario && 
                uaec.CodigoTipoPerfilAcesso == (short)Enums.TipoPerfilAcessoUsuarioEnum.Responsavel &&
                uaec.DataInicialVigenciaAcessoUsuario <= DateTime.Now && 
                (uaec.DataFinallVigenciaAcessoUsuario == null || uaec.DataFinallVigenciaAcessoUsuario >= DateTime.Now))
                .Select(c => c.EntidadeControle).Distinct().FirstOrDefault();
        }
    }
}
