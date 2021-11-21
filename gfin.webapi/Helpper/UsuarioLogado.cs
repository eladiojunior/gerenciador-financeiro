using System;
using gfin.webapi.Negocio.Listeners;

namespace gfin.webapi.Api.Helpper
{
    public class UsuarioLogado : IUsuarioLogado
    {
        public UsuarioLogado() {}

        public bool IsLogado => true;
        public int IdUsuario => 1;
        public string NomeUsuario => "Api";
        public string EmailUsuario => "eladiojunior@gmail.com";
        public DateTime DataUltimoAcessoUsuario => DateTime.Now;
        public int IdEntidade => 1;
        public string NomeEntidade => "Pessoal";
        public bool IsEmpresa => false;
        public string FotoBase64 => null;
        public short CodigoPerfil => 1;
        public string NomePerfil => "Administrador";
    }
}