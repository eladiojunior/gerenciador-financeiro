namespace gfin.webapi.Negocio.Listeners
{
    /// <summary>
    /// Padrão 'Listener' para recuperar as informações da aplicação cliente (Usuario Logado, Mobile, 
    /// Navegador, Versão, IP, etc.) sem a necessidade de passar essas informações diretamente, 
    /// minimizando o acoplamento das camadas.
    /// @autor Eladio Júnior
    /// @data 09/11/2016
    /// </summary>
    public interface IClienteListener
    {
        string IpMaquinaUsuario { get; }
        bool IsDispositivoMobileUsuario { get; }
        string InfoDispositivoUsuario { get; }
        IUsuarioLogado UsuarioLogado { get; }
        string MapPathAppServidor { get; }
    }
}
