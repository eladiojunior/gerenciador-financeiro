using gfin.webapi.Dados.Models;

namespace gfin.webapi.Dados
{
    internal class UsuarioSistemaDAO : GenericDAO<UsuarioSistema>
    {
        internal UsuarioSistemaDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
