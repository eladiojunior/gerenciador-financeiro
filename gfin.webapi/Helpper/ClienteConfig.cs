using gfin.webapi.Negocio.Listeners;

namespace gfin.webapi.Api.Helpper
{
    public class ClienteConfig : IClienteListener
    {
        public ClienteConfig() {}
        
        public string IpMaquinaUsuario => "0.0.0.0";
        public bool IsDispositivoMobileUsuario => false;

        public string InfoDispositivoUsuario => "API";

        public IUsuarioLogado UsuarioLogado => new UsuarioLogado();

        public string MapPathAppServidor => "";
    }
}