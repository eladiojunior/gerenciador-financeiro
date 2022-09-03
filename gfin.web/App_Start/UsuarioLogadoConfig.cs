using GFin.Dados.Enums;
using GFin.Dados.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace GFin.Web
{
    internal class UsuarioLogadoWeb : GFin.Negocio.Listeners.IUsuarioLogado 
    {
        private bool _isLogado = false;
        private int _idUsuario = 0;
        private string _nome = string.Empty;
        private string _email = string.Empty;
        private DateTime _dataUltimoAcesso = DateTime.Now;
        private int _idEntidade = 0;
        private string _nomeEntidade = string.Empty;
        private bool _isEmpresa = false;
        private string _fotoBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFsAAABbCAMAAAAr6AmrAAAAMFBMVEXFxcX+/v7CwsLIyMjNzc3V1dXh4eHm5ubs7Ozx8fH6+vrp6en09PT39/fc3NzS0tKCih/2AAACAElEQVRoge2Ya46DMAyEifOEpnD/2y6I3S1t42RiU6k/+A4wsgbHHjMMFxcXFxdfAO18Qtj5EGMM3p0sb5dk/pniYk9TdiGbZ3JwZwiTnUfzzjhbtTXk0lSQXp25aY0hVyp6JzlV5bTw0qsvGvFa1Vpxm6rSxsg9t7EhbUwQStNS7pAj9+VjZRsTRa4gZa9tLiscKXstXCJt6/33xygxxUPSxvh+aQqgduh/P/Q6VzmS4G0iXbIx9UsPoLQxl/bXa6N9kgX9jT35dT8ItNF3OXdLDwM0YoVD9pNzEJzfso3p7oB0FgZD5GsKFz0BhWdp+qG5qT3Lg1XLFWn02bC3qrQuJrtaIky644Eq4SrqL4e5vJTzPGilieZyJ2avvAXJ+pEbWVP2KlN8fV4lQab6pdGB+/eUCK92IMlqktyZFt073aVT9dE803ln9kh3irfuyndxXLp5V76J4x8U25RH0KsH7pAjAesWj6a1IxPyRMmhx8gzyMKn9kMv086FhJ5+7ywNcbIyRzbGViO2YwNPI1BYJKdx3Kt9SP2v5kisFQ7FS55aH8K3Akfltu+eUa8k1nHyOkvWr+m5wqEfVHW4kdW7EUqwW0JtyWYKo63tkg0mkZ9gN5soTrCbNXw5QZo7k+WT+0j5Y2rG64Pyr4MzPiXXKNphsnMrSSu22ZH8aMIfkDkUmuoPguQAAAAASUVORK5CYII=";
        private short _codigoPerfil = 0;
        private string _nomePerfil = string.Empty;
        public bool IsLogado
        {
            get { return _isLogado; }
            set { _isLogado = value; }
        }

        public int IdUsuario
        {
            get { return _idUsuario; }
            set { _idUsuario = value; }
        }

        public string NomeUsuario
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public string EmailUsuario
        {
            get { return _email; }
            set { _email = value; }
        }

        public DateTime DataUltimoAcessoUsuario
        {
            get { return _dataUltimoAcesso; }
            set { _dataUltimoAcesso = value; }
        }

        public int IdEntidade
        {
            get { return _idEntidade; }
            set { _idEntidade = value; }
        }

        public string NomeEntidade
        {
            get { return _nomeEntidade; }
            set { _nomeEntidade = value; }
        }
        public bool IsEmpresa
        {
            get { return _isEmpresa; }
            set { _isEmpresa = value; }
        }
        public string FotoBase64
        {
            get { return _fotoBase64; }
            set { 
                string strValue = value;
                if (!strValue.Contains("data:image/png;base64,"))
                    strValue = string.Format("data:image/png;base64,{0}", strValue);
                _fotoBase64 = strValue;
            }
        }
        public short CodigoPerfil
        {
            get { return _codigoPerfil; }
            set { _codigoPerfil = value; }
        }
        public string NomePerfil
        {
            get { return _nomePerfil; }
            set { _nomePerfil = value; }
        }
    }

    public class UsuarioLogadoConfig : GFin.Negocio.Listeners.IClienteListener 
    {
        private const string KeySessionUsuario = "KEY_SESSION_USUARIO";
        private static UsuarioLogadoConfig _instancia = null;
        
        private UsuarioLogadoConfig() {}
        
        public static UsuarioLogadoConfig Instance { 
            get {
                return _instancia ?? new UsuarioLogadoConfig();
            }
        }

        /// <summary>
        /// Atualiza os dados do usuário logado na SESSION após uma alteração dos dados cadastrais.
        /// </summary>
        /// <param name="nomeUsuario">Nome do usuário alterado.</param>
        /// <param name="emailUsuario">E-mail do usuário alterado.</param>
        /// <param name="nomeEntidadeControle">Nome da entidade de controle.</param>
        public void AtualizarDadosUsuario(string nomeUsuario, string emailUsuario, string nomeEntidadeControle, string fotoBase64 = null)
        {
            UsuarioLogadoWeb _usuarioLogado = this.UsuarioLogado as UsuarioLogadoWeb;
            _usuarioLogado.NomeUsuario = nomeUsuario;
            _usuarioLogado.EmailUsuario = emailUsuario;
            _usuarioLogado.NomeEntidade = nomeEntidadeControle;
            if (fotoBase64 != null)
                _usuarioLogado.FotoBase64 = fotoBase64;
            this.UsuarioLogado = _usuarioLogado;
        }

        /// <summary>
        /// Atualiza a foto do usuário logado na SESSION após uma alteração.
        /// </summary>
        /// <param name="fotoBase64">Foto na base64 Hexdecimal.</param>
        public void AtualizarFotoUsuario(string fotoBase64)
        {
            UsuarioLogadoWeb _usuarioLogado = this.UsuarioLogado as UsuarioLogadoWeb;
            _usuarioLogado.FotoBase64 = fotoBase64;
            this.UsuarioLogado = _usuarioLogado;
        }

        /// <summary>
        /// Registra o lokgin na session do usuário e componente de autenticação (FormsAuthentication);
        /// </summary>
        /// <param name="usuarioAcessoEntidade">Informações do usuário autenticado.</param>
        /// <param name="HasManterConectado">Manter o usuário conectado utilizando cookie.</param>
        public void RegistrarLogin(UsuarioAcessoEntidadeControle usuarioAcessoEntidade, bool HasManterConectado)
        {
            
            UsuarioLogadoWeb usuarioLogado = CriarUsuarioLogadoWeb(usuarioAcessoEntidade);

            var infoUsuario = JsonConvert.SerializeObject(new { IdUsuario = usuarioLogado.IdUsuario, IdEntidade = usuarioLogado.IdEntidade });
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, usuarioLogado.EmailUsuario, DateTime.Now, DateTime.Now.AddMinutes(30), HasManterConectado, infoUsuario, FormsAuthentication.FormsCookiePath);
            
            FormsAuthentication.RedirectFromLoginPage(usuarioLogado.EmailUsuario, HasManterConectado);
            if (HasManterConectado)
            {//Atualizar cookie...
                var authCookie = FormsAuthentication.GetAuthCookie(usuarioLogado.EmailUsuario, HasManterConectado);
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, infoUsuario, ticket.CookiePath);
                var encTicket = FormsAuthentication.Encrypt(newTicket);
                authCookie.Value = encTicket;
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }

            //Registrar usuário logado...
            this.UsuarioLogado = usuarioLogado;
            
        }

        /// <summary>
        /// Criar uma instância do usuário logado no sistema para coloca-lo na session.
        /// </summary>
        /// <param name="usuarioAcessoEntidade">Informações do usuário para criação da nova instância.</param>
        /// <returns></returns>
        private UsuarioLogadoWeb CriarUsuarioLogadoWeb(UsuarioAcessoEntidadeControle usuarioAcessoEntidade)
        {
            UsuarioLogadoWeb usuarioLogado = new UsuarioLogadoWeb();
            usuarioLogado.IsLogado = true;
            usuarioLogado.IdUsuario = usuarioAcessoEntidade.UsuarioAcesso.Id;
            usuarioLogado.EmailUsuario = usuarioAcessoEntidade.UsuarioAcesso.EmailUsuario;
            usuarioLogado.NomeUsuario = usuarioAcessoEntidade.UsuarioAcesso.NomeUsuario;
            usuarioLogado.IdEntidade = usuarioAcessoEntidade.EntidadeControle.Id;
            usuarioLogado.NomeEntidade = usuarioAcessoEntidade.EntidadeControle.NomeEntidade;
            usuarioLogado.IsEmpresa = (usuarioAcessoEntidade.EntidadeControle.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Juridica);
            usuarioLogado.CodigoPerfil = usuarioAcessoEntidade.CodigoTipoPerfilAcesso;
            usuarioLogado.NomePerfil = UtilEnum.GetTextoTipoPerfilAcessoUsuario(usuarioAcessoEntidade.CodigoTipoPerfilAcesso);
            if (!usuarioAcessoEntidade.UsuarioAcesso.DataUltimoAcessoUsuario.HasValue)
                usuarioAcessoEntidade.UsuarioAcesso.DataUltimoAcessoUsuario = DateTime.Now;
            usuarioLogado.DataUltimoAcessoUsuario = usuarioAcessoEntidade.UsuarioAcesso.DataUltimoAcessoUsuario.Value;
            if (usuarioAcessoEntidade.UsuarioAcesso.FotoUsuario != null)
            {
                string _fotoBase64 = System.Convert.ToBase64String(usuarioAcessoEntidade.UsuarioAcesso.FotoUsuario, 0, usuarioAcessoEntidade.UsuarioAcesso.FotoUsuario.Length);
                usuarioLogado.FotoBase64 = _fotoBase64;
            }
            return usuarioLogado;
        }

        /// <summary>
        /// Informa se o usuário encontra-se logado na aplicação.
        /// </summary>
        public bool HasUsuarioLogado()
        {
            return (this.UsuarioLogado != null && UsuarioLogado.IdUsuario != 0);
        }

        /// <summary>
        /// Efetua o logoff do usuário.
        /// </summary>
        public void Logoff()
        {
            FormsAuthentication.SignOut();
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Abandon();
            }
            if (HttpContext.Current.Response != null)
            {
                HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        /// <summary>
        /// Recupera o Ip da Máquina Cliente do Usuário.
        /// </summary>
        /// <returns></returns>
        public string IpMaquinaUsuario
        {
            get
            {
                string ipMaquina = string.Empty;
                try
                {
                    ipMaquina = HttpContext.Current.Request.UserHostAddress;
                    if (string.IsNullOrEmpty(ipMaquina) || ipMaquina.Equals("::1"))
                        ipMaquina = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        
                }
                //Erro ao recuperar IP da máquina cliente.
                catch { }
                return ipMaquina;
            }
        }
        
        /// <summary>
        /// Retorna de o usuário está acessando a aplicação de um dispositivo Mobile.
        /// </summary>
        public bool IsDispositivoMobileUsuario
        {
            get 
            {
                bool isMobile = false;
                try
                {
                    isMobile = HttpContext.Current.Request.Browser.IsMobileDevice;
                }
                //Erro ao recuperar as informações de dispositivo Mobile cliente.
                catch { }
                return isMobile; 
            }
        }

        /// <summary>
        /// Reuni as informações do dispositivo retirados da request (Request.ServerVariables["HTTP_USER_AGENT"]) do cliente.
        /// </summary>
        public string InfoDispositivoUsuario
        {
            get
            {
                string infoDispositivo = string.Empty;
                try
                {
                    infoDispositivo = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                }
                //Erro ao recuperar as informações do dispositivo cliente.
                catch { }
                return infoDispositivo;
            }
        }

        public Negocio.Listeners.IUsuarioLogado UsuarioLogado
        {
            get
            {
                return ObterUsuarioLogado();
            }
            set
            {
                HttpContext.Current.Session.Add(KeySessionUsuario, value);
            }
        }

        /// <summary>
        /// Recupera a instância do usuário logado na aplicação.
        /// Será verificado o usuário na sessão, assim como se foi persistido seu acesso em cookie.
        /// </summary>
        /// <returns></returns>
        private Negocio.Listeners.IUsuarioLogado ObterUsuarioLogado()
        {
            UsuarioLogadoWeb _usuarioLogado = ObterUsuarioSession();
            if (_usuarioLogado == null)
            {
                //Verificar se o usuário está registrado no cookie.
                _usuarioLogado = ObterUsuarioAuthenticationTicket();
            }
            if (_usuarioLogado != null)
            {
                this.UsuarioLogado = _usuarioLogado;
                return _usuarioLogado;
            }
            return new UsuarioLogadoWeb();
        }

        /// <summary>
        /// Recupera o usuário do cookie [FormsAuthenticationTicket] caso ainda estiver válido.
        /// </summary>
        /// <returns></returns>
        private UsuarioLogadoWeb ObterUsuarioAuthenticationTicket()
        {
            UsuarioLogadoWeb _usuarioLogado = new UsuarioLogadoWeb();
            FormsAuthenticationTicket authTicket = null;
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket != null &&
                !authTicket.Expired &&
                !string.IsNullOrEmpty(authTicket.UserData))
            {//Recupera usuário e verificar validade dos dados do usuário...
                Dictionary<string, int> infoUsuario = JsonConvert.DeserializeObject<Dictionary<string, int>>(authTicket.UserData);
                int idUsuario = infoUsuario["IdUsuario"];
                int idEntidade = infoUsuario["IdEntidade"];
                GFin.Negocio.UsuarioNegocio usuarioNegocio = new Negocio.UsuarioNegocio(this);
                UsuarioAcessoEntidadeControle _usuarioAcessoEntidade = usuarioNegocio.ObterUsuarioAcessoEntidade(idEntidade, idUsuario, true);
                if (_usuarioAcessoEntidade != null && 
                    _usuarioAcessoEntidade.UsuarioAcesso != null && 
                    _usuarioAcessoEntidade.UsuarioAcesso.EmailUsuario.Equals(authTicket.Name))
                {
                    usuarioNegocio.GravarHistoricoUsuario(idUsuario, TipoOperacaoHistoricoUsuarioEnum.AutenticarUsuario, string.Format("Autenticação automática por cookie, válido até {0}.", authTicket.Expiration.ToString("dd/MM/yyyy HH:mm:ss")));
                    _usuarioLogado = CriarUsuarioLogadoWeb(_usuarioAcessoEntidade);
                    this.UsuarioLogado = _usuarioLogado;
                    FormsAuthentication.RenewTicketIfOld(authTicket);
                }
            }
            return _usuarioLogado;
        }

        /// <summary>
        /// Recupera o usuário da session caso exista.
        /// </summary>
        /// <returns></returns>
        private UsuarioLogadoWeb ObterUsuarioSession()
        {
            return HttpContext.Current.Session[KeySessionUsuario] as UsuarioLogadoWeb;
        }

        /// <summary>
        /// Acionar a tela de login do usuário.
        /// </summary>
        internal void Login()
        {
            FormsAuthentication.RedirectToLoginPage();
        }

        /// <summary>
        /// Verifica se o usuário logado possue o perfil definido no atributo 'roles'.
        /// Podem haver mais de um role, será uma string de array, separada por ';'.
        /// </summary>
        /// <param name="roles">Roles definidos na interface para verificação.</param>
        /// <returns></returns>
        public bool IsPerfil(string roles)
        {
            var result = false;
            if (this.HasUsuarioLogado())
                result = roles.Contains(this.UsuarioLogado.CodigoPerfil.ToString());
            return result;
        }

        /// <summary>
        /// Verifica se o usuário logado possue o perfil definido no atributo 'enums'.
        /// Podem haver mais de um perfil, será um array de objetos 'TipoPerfilAcessoUsuarioEnum'.
        /// </summary>
        /// <param name="enums">Perfis definidos na interface para verificação.</param>
        /// <returns></returns>
        public bool IsPerfil(params TipoPerfilAcessoUsuarioEnum[] enums)
        {
            var result = false;
            if (this.HasUsuarioLogado())
                foreach (var item in enums)
                {
                    result = this.UsuarioLogado.CodigoPerfil == (short)item;
                    if (result) break;
                }
            return result;
        }

        public string MapPathAppServidor
        {
            get {
                return HttpContext.Current.Server.MapPath("/");
            }
        }
    }
}