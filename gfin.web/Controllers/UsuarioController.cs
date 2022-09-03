using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GFin.Web.Controllers
{
    internal class InfoImagemFoto
    {
        [JsonProperty("x")]
        public float X { get; set; }
        [JsonProperty("y")]
        public float Y { get; set; }
        [JsonProperty("height")]
        public float Height { get; set; }
        [JsonProperty("width")]
        public float Width { get; set; }
        [JsonProperty("rotate")]
        public float Rotate { get; set; }
    }

    public class UsuarioController : GenericController
    {
        //
        // GET: /Usuario/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginUsuarioModel model = new LoginUsuarioModel();
            model.DropboxEntidades = DropboxEntidadePadrao();
            model.RetornoUrl = ObterUrlRetornoAposLogin();
            return View(model);
        }

        /// <summary>
        /// Recupera a Url de retorno apos o Login ser realizado.
        /// </summary>
        /// <returns></returns>
        private string ObterUrlRetornoAposLogin()
        {
            string urlReturn = System.Web.Helpers.WebCache.Get("returnUrl") as string;
            System.Web.Helpers.WebCache.Remove("returnUrl");
            return urlReturn;
        }

        /// <summary>
        /// Criar Dropbox de Entidade Controle padrão.
        /// </summary>
        /// <returns></returns>
        private Models.Helpers.DropboxModel DropboxEntidadePadrao()
        {
            return new Models.Helpers.DropboxModel()
            {
                Itens = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Informe seu login.", Value = "", Selected = true}
                }
            };
        }

        /// <summary>
        /// Criar dropbox de Entidades Controle do usuário informado (e-mail), selecionado o Id da Entidade informada.
        /// </summary>
        /// <param name="email">E-mail do usuário para carregar a dropbox.</param>
        /// <param name="idEntidadeSelecionada">Identificador da Entidade selecionada.</param>
        /// <returns></returns>
        private Models.Helpers.DropboxModel DropboxEntidadeUsuario(string email, int idEntidadeSelecionada)
        {
            Models.Helpers.DropboxModel dropbox = new Models.Helpers.DropboxModel();
            EntidadeControleNegocio negocioEntidade = new EntidadeControleNegocio(UsuarioLogadoConfig.Instance);
            var listaEntidadesLogin = negocioEntidade.ListarEntidadeControle(email);
            dropbox.Itens = listaEntidadesLogin.Select(c => new SelectListItem() { Text = c.NomeEntidade, Value = c.Id.ToString(), Selected = (c.Id==idEntidadeSelecionada) }).ToList();
            return dropbox;
        }

        /// <summary>
        /// Action, Json, que recupera a lista de entidades de controle de um usuário pelo login.
        /// </summary>
        /// <param name="login">Login do usuário para verificação e recuperação de suas entidades.</param>
        /// <returns></returns>
        // GET: /Usuario/JsonListarEntidades
        [HttpGet]
        [AllowAnonymous]
        public JsonResult JsonListarEntidades(string login)
        {
            List<SelectListItem> resultLista = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(login))
            {
                EntidadeControleNegocio negocioEntidade = new EntidadeControleNegocio(UsuarioLogadoConfig.Instance);
                var listaEntidadesLogin = negocioEntidade.ListarEntidadeControle(login);
                resultLista = listaEntidadesLogin.Select(c => new SelectListItem() { Text = c.NomeEntidade, Value = c.Id.ToString() }).ToList();
            }
            return Json(resultLista, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Usuario/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUsuarioModel model)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Login", "E-mail e/ou senha informados é inválido.");
                model.DropboxEntidades = DropboxEntidadeUsuario(model.Email, model.IdEntidade);
                return View(model);
            }

            UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);

            //Verificar se o usuário já está registrado, mas não confirmou o registro.
            UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.Email);
            if (_usuarioRegistrado != null && _usuarioRegistrado.IsConfirmacaoEmailUsuario == false)
            {//Usuário registrado e sem confirmação de registro.
                ReenviarConfirmacaoUsuarioModel modelReenviar = new ReenviarConfirmacaoUsuarioModel();
                modelReenviar.IdUsuario = _usuarioRegistrado.Id;
                modelReenviar.Nome = _usuarioRegistrado.NomeUsuario;
                modelReenviar.Email = _usuarioRegistrado.EmailUsuario;
                modelReenviar.DataHoraRegistro = _usuarioRegistrado.DataHoraRegistroUsuario;
                return View("ReenviarConfirmacaoUsuario", modelReenviar);
            }

            _usuarioRegistrado = negocioUsuario.Autenticar(model.Email, model.Senha);
            if (_usuarioRegistrado == null)
            {
                ModelState.AddModelError("Login", "E-mail e/ou senha informados é inválido.");
                model.DropboxEntidades = DropboxEntidadeUsuario(model.Email, model.IdEntidade);
                return View(model);
            }

            UsuarioAcessoEntidadeControle _usuarioAcessoEntidade = negocioUsuario.ObterUsuarioAcessoEntidade(model.IdEntidade, _usuarioRegistrado.Id);
            if (_usuarioAcessoEntidade == null)
            {
                ModelState.AddModelError("Login", "Não foi possível identificar a entidade de controle do usuário.");
                model.DropboxEntidades = DropboxEntidadeUsuario(model.Email, model.IdEntidade);
                return View(model);
            }
            _usuarioAcessoEntidade.UsuarioAcesso = _usuarioRegistrado;

            //Registrar autenticação do usuário.
            UsuarioLogadoConfig.Instance.RegistrarLogin(_usuarioAcessoEntidade, model.ManterConectado);

            return RedirectToLocal(model.RetornoUrl);

        }

        //
        // GET: /Usuario/LogOff
        [HttpGet]
        public ActionResult LogOff()
        {
            UsuarioLogadoConfig.Instance.Logoff();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Usuario/Cadastrar
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Cadastrar()
        {
            return View();
        }

        //
        // POST: /Usuario/Registrar
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(UsuarioRegistroModel model)
        {
            
            if (!ModelState.IsValid) {
                return View("Cadastrar");
            }

            try
            {

                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);
            
                //Verificar se o usuário já está registrado, mas não confirmou o registro.
                UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.Email);
                if (_usuarioRegistrado != null && _usuarioRegistrado.IsConfirmacaoEmailUsuario == false)
                {//Usuário registrado e sem confirmação de registro.
                    ReenviarConfirmacaoUsuarioModel modelReenviar = new ReenviarConfirmacaoUsuarioModel();
                    //Obter a entidade
                    modelReenviar.IdUsuario = _usuarioRegistrado.Id;
                    modelReenviar.Nome = _usuarioRegistrado.NomeUsuario;
                    modelReenviar.Email = _usuarioRegistrado.EmailUsuario;
                    modelReenviar.DataHoraRegistro = _usuarioRegistrado.DataHoraRegistroUsuario;
                    return View("ReenviarConfirmacaoUsuario", modelReenviar);
                }

                Dados.Models.UsuarioAcessoEntidadeControle usuarioAcessoEntidade = new Dados.Models.UsuarioAcessoEntidadeControle();
                usuarioAcessoEntidade.CodigoTipoPerfilAcesso = (short)Dados.Enums.TipoPerfilAcessoUsuarioEnum.Responsavel;
                usuarioAcessoEntidade.DataFinallVigenciaAcessoUsuario = null;
                usuarioAcessoEntidade.DataInicialVigenciaAcessoUsuario = DateTime.Now;
                usuarioAcessoEntidade.DataRegistroUsuarioAcesso = DateTime.Now;
            
                Dados.Models.EntidadeControle entidade = new Dados.Models.EntidadeControle();
                entidade.CodigoTipoEntidade = model.CodigoTipoEntidadeControle;
                entidade.NomeEntidade = (model.CodigoTipoEntidadeControle == (short)Dados.Enums.TipoEntidadeControleEnum.Fisica ? "Minha Casa" : model.NomeEntidadeControle);
                entidade.CpfCnpjEntidade = model.CpfCnpjEntidadeControle;
                entidade.CodigoTipoSituacaoEntidade = (short)Dados.Enums.TipoSituacaoEnum.Ativo;
                usuarioAcessoEntidade.EntidadeControle = entidade;

                Dados.Models.UsuarioSistema usuarioAcesso = new Dados.Models.UsuarioSistema();
                usuarioAcesso.NomeUsuario = model.Nome;
                usuarioAcesso.SenhaUsuario = model.Senha;
                usuarioAcesso.EmailUsuario = model.Email;
                usuarioAcesso.DataHoraRegistroUsuario = DateTime.Now;
                usuarioAcesso.CodigoTipoSituacaoUsuario = (short)Dados.Enums.TipoSituacaoEnum.Ativo;
                usuarioAcessoEntidade.UsuarioAcesso = usuarioAcesso;

                negocioUsuario.RegistrarUsuario(usuarioAcessoEntidade);

                TempData["NomeUsuario"] = model.Nome;
                TempData["EmailUsuario"] = model.Email;
                return RedirectToAction("Registrado");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "RegistrarUsuario");
                return View("Cadastrar");
            }

        }

        //
        // GET: /Usuario/Registrado
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registrado()
        {

            var nomeUsuario = TempData["NomeUsuario"] as string;
            if (string.IsNullOrEmpty(nomeUsuario))
                return RedirectToAction("Login");

            UsuarioRegistradoModel model = new UsuarioRegistradoModel();
            model.Nome = TempData["NomeUsuario"].ToString();
            model.Email = TempData["EmailUsuario"].ToString();
            return View(model);

        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Usuario/ConfirmacaoUsuario
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmacaoUsuario(string token)
        {
            const string msgErroUsuario = "Desculpe, ocorreu algum problema na confirmação de seu registro. Por gentileza, poderia acessar seu e-mail e clicar novamente no link enviado [Confirmação de e-mail].";
            ConfirmacaoUsuarioModel model = new ConfirmacaoUsuarioModel();
            model.IsConfirmado = false;
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, msgErroUsuario);
                return View(model);
            }
            
            try
            {
                model.Token = token;
                //Recupera identificador do usuário acesso entidade no token...
                int idUsuario = Int32.Parse(UtilNegocio.Descriptografar(token));
                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);
                UsuarioSistema _usuarioAcesso = negocioUsuario.ObterUsuarioSistema(idUsuario);
                if (_usuarioAcesso == null)
                {//Usuário Acesso não identificado...
                    ModelState.AddModelError(string.Empty, msgErroUsuario);
                    return View(model);
                }
                if (_usuarioAcesso.IsConfirmacaoEmailUsuario)
                {//Confirmação já realizada...
                    ModelState.AddModelError(string.Empty, "Confirmação de registro já realizada, faça seu login para acessar a aplicação.");
                    return View(model);
                }

                //Gravar confirmação de e-mail do usuário.
                _usuarioAcesso.IsConfirmacaoEmailUsuario = true;
                negocioUsuario.ConfirmarEmailUsuario(_usuarioAcesso);
                model.IsConfirmado = true;
                model.Nome = _usuarioAcesso.NomeUsuario;
                //Recupera a Entidade de Controle no qual o usuário é responsável...
                EntidadeControleNegocio negocioEntidadeControle = new EntidadeControleNegocio(UsuarioLogadoConfig.Instance);
                EntidadeControle entidadeResponsavel = negocioEntidadeControle.ObterEntidadeControlePadraoDoUsuario(idUsuario);
                //Criptografar, IdUsuario, IdEntidade e E-mail do usuário para melhorar a segurança da autenticação via Token.
                model.Token = UtilNegocio.Criptografar(string.Format("{0}|{1}|{2}", _usuarioAcesso.Id, (entidadeResponsavel == null ? 0 : entidadeResponsavel.Id), _usuarioAcesso.EmailUsuario));
            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, string.Format("ConfirmacaoUsuario({0})", token));
                ModelState.AddModelError(string.Empty, msgErroUsuario);
            }

            return View(model);
        }

        //
        // POST: /Usuario/ReenviarConfirmacaoEmail
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ReenviarConfirmacaoEmail(ReenviarConfirmacaoUsuarioModel model)
        {
            try
            {
                
                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);

                UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.IdUsuario);
                if (_usuarioRegistrado != null)
                {

                    model.IdUsuario = _usuarioRegistrado.Id;
                    model.Nome = _usuarioRegistrado.NomeUsuario;
                    model.Email = _usuarioRegistrado.EmailUsuario;
                    model.DataHoraRegistro = _usuarioRegistrado.DataHoraRegistroUsuario;

                    if (_usuarioRegistrado.IsConfirmacaoEmailUsuario)
                    {//Confirmação já realizada...
                        ModelState.AddModelError(string.Empty, "Confirmação de registro já realizada, faça seu login para acessar a aplicação.");
                    }
                    else
                    {

                        //Registrar histórico de reenvio de confirmação de e-mail.
                        negocioUsuario.GravarHistoricoUsuario(_usuarioRegistrado.Id,
                                TipoOperacaoHistoricoUsuarioEnum.ReenviarConfirmacaoEmailUsuario,
                                "Usuário solicita reenvio da confirmação do registrado.");

                        negocioUsuario.EnviarSolicitacaoConfirmacaoEmailUsuario(_usuarioRegistrado);
                        return View("Reenviado", model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário não localizado para reenvio da confirmação de registro.");
                }

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "ReenvioConfirmacaoUsuario");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível reenviar sua confirmação de registro.");
            }

            return View("ReenviarConfirmacaoUsuario", model);

        }

        //
        // GET: /Usuario/LoginToken
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoginToken(string token)
        {

            const string msgErroToken = "Desculpe, ocorreu algum problema ao tentar realizar seu login via token. Por gentileza, realize o login informando seu e-mail e senha.";
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("Login", msgErroToken);
                return View("Login");
            }
            string tokenDescriptografado = UtilNegocio.Descriptografar(token);
            string[] arrayToken = tokenDescriptografado.Split('|');
            if (arrayToken.Length != 3)
            {
                ModelState.AddModelError("Login", msgErroToken);
                return View("Login");
            }
            
            int idUsuario = Int32.Parse(arrayToken[0]);
            int idEntidade = Int32.Parse(arrayToken[1]);
            string emailUsuario = arrayToken[2];
            
            UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);
            UsuarioAcessoEntidadeControle _usuarioAcessoEntidade = negocioUsuario.ObterUsuarioAcessoEntidade(idEntidade, idUsuario, true);
            if (_usuarioAcessoEntidade == null || !emailUsuario.Equals(_usuarioAcessoEntidade.UsuarioAcesso.EmailUsuario, StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError("Login", msgErroToken);
                return View("Login");
            }

            //Registrar autenticação do usuário.
            UsuarioLogadoConfig.Instance.RegistrarLogin(_usuarioAcessoEntidade, false);

            //Registrar histórico de login por token.
            negocioUsuario.GravarHistoricoUsuario(idUsuario, TipoOperacaoHistoricoUsuarioEnum.AutenticarUsuario, 
                string.Format("Autenticação do usuário por token [{0}].", token));

            return RedirectToAction("Index", "Home");

        }

        //
        // GET: /Usuario/EsqueciMinhaSenha
        [HttpGet]
        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha()
        {
            return View();
        }

        // POST: /Usuario/RecuperarSenha
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecuperarSenha(UsuarioRecuperarSenhaModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("EsqueciMinhaSenha");
            }

            try
            {

                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);

                UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.Email);
                if (_usuarioRegistrado == null)
                {
                    ModelState.AddModelError(string.Empty, "Desculpe seu e-mail não foi encontrado, por favor verifique.");
                    return View("EsqueciMinhaSenha");
                }

                if (!_usuarioRegistrado.IsConfirmacaoEmailUsuario)
                {//Usuário não confirmação seu registro...
                    ReenviarConfirmacaoUsuarioModel modelReenviar = new ReenviarConfirmacaoUsuarioModel();
                    modelReenviar.IdUsuario = _usuarioRegistrado.Id;
                    modelReenviar.Nome = _usuarioRegistrado.NomeUsuario;
                    modelReenviar.Email = _usuarioRegistrado.EmailUsuario;
                    modelReenviar.DataHoraRegistro = _usuarioRegistrado.DataHoraRegistroUsuario;
                    ModelState.AddModelError(string.Empty, "Identificamos que você ainda não confirmou seu registro.");
                    return View("ReenviarConfirmacaoUsuario", modelReenviar);
                }
                
                model.IdUsuario = _usuarioRegistrado.Id;
                model.Email = _usuarioRegistrado.EmailUsuario;
                
                //Gerar codigo de segurança para recuperação do acesso...
                string codigoSeguranca = UtilNegocio.GerarPasswordRandom();

                negocioUsuario.EnviarSolicitacaoRecuperacaoSenhaUsuario(_usuarioRegistrado.NomeUsuario, _usuarioRegistrado.EmailUsuario, codigoSeguranca);

                //Registrar histórico de solicitação de recuperação de senha acesso.
                negocioUsuario.GravarHistoricoUsuario(_usuarioRegistrado.Id,
                        TipoOperacaoHistoricoUsuarioEnum.RecuperarSenhaAcesso, codigoSeguranca);

                //Alterar indicação de alteração de senha, isso fará com que o usuário altere sua senha após login.
                _usuarioRegistrado.IsAlterarSenhaUsuario = true;
                negocioUsuario.GravarUsuarioSistema(_usuarioRegistrado);

                return View("ConfirmarCodigoSeguranca", model);

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "RecuperarSenha");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível validar seu e-mail para recuperação de sua senha.");
                return View("EsqueciMinhaSenha");
            }

        }

        // POST: /Usuario/ConfirmarCodigoSeguranca
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ConfirmarCodigoSeguranca(UsuarioRecuperarSenhaModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("ConfirmarCodigoSeguranca", model);
            }

            try
            {

                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);

                UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.IdUsuario);
                if (_usuarioRegistrado == null || !_usuarioRegistrado.EmailUsuario.Equals(model.Email)) {
                    ModelState.AddModelError(string.Empty, "Desculpe tivemos problemas em identificar seu usuário registrado.");
                    return View("ConfirmarCodigoSeguranca", model);
                }

                //Validar o código de recuperação de acesso do usuário.
                if (!negocioUsuario.ValidarCodigoRecuperacaoAcessoUsuario(model.IdUsuario, model.Email, model.CodigoSeguranca)) {
                    ModelState.AddModelError(string.Empty, "Desculpe o código de recuperação do acesso não confere. Verifique seu e-mail e copie novamente o código enviado para você.");
                    return View("ConfirmarCodigoSeguranca", model);
                }

                //Código validado, solicitar redefinição de senha do usuário.
                RedefinirSenhaUsuarioModel modelRedefinirSenha = new RedefinirSenhaUsuarioModel();
                modelRedefinirSenha.IdUsuario = _usuarioRegistrado.Id;
                modelRedefinirSenha.Nome = _usuarioRegistrado.NomeUsuario;
                modelRedefinirSenha.Email = _usuarioRegistrado.EmailUsuario;
                string _fotoBase64 = "iVBORw0KGgoAAAANSUhEUgAAAFsAAABbCAMAAAAr6AmrAAAAMFBMVEXFxcX+/v7CwsLIyMjNzc3V1dXh4eHm5ubs7Ozx8fH6+vrp6en09PT39/fc3NzS0tKCih/2AAACAElEQVRoge2Ya46DMAyEifOEpnD/2y6I3S1t42RiU6k/+A4wsgbHHjMMFxcXFxdfAO18Qtj5EGMM3p0sb5dk/pniYk9TdiGbZ3JwZwiTnUfzzjhbtTXk0lSQXp25aY0hVyp6JzlV5bTw0qsvGvFa1Vpxm6rSxsg9t7EhbUwQStNS7pAj9+VjZRsTRa4gZa9tLiscKXstXCJt6/33xygxxUPSxvh+aQqgduh/P/Q6VzmS4G0iXbIx9UsPoLQxl/bXa6N9kgX9jT35dT8ItNF3OXdLDwM0YoVD9pNzEJzfso3p7oB0FgZD5GsKFz0BhWdp+qG5qT3Lg1XLFWn02bC3qrQuJrtaIky644Eq4SrqL4e5vJTzPGilieZyJ2avvAXJ+pEbWVP2KlN8fV4lQab6pdGB+/eUCK92IMlqktyZFt073aVT9dE803ln9kh3irfuyndxXLp5V76J4x8U25RH0KsH7pAjAesWj6a1IxPyRMmhx8gzyMKn9kMv086FhJ5+7ywNcbIyRzbGViO2YwNPI1BYJKdx3Kt9SP2v5kisFQ7FS55aH8K3Akfltu+eUa8k1nHyOkvWr+m5wqEfVHW4kdW7EUqwW0JtyWYKo63tkg0mkZ9gN5soTrCbNXw5QZo7k+WT+0j5Y2rG64Pyr4MzPiXXKNphsnMrSSu22ZH8aMIfkDkUmuoPguQAAAAASUVORK5CYII=";
                if (_usuarioRegistrado.FotoUsuario != null)
                {
                    _fotoBase64 = System.Convert.ToBase64String(_usuarioRegistrado.FotoUsuario, 0, _usuarioRegistrado.FotoUsuario.Length);
                }
                modelRedefinirSenha.FotoBase64 = string.Format("data:image/png;base64,{0}", _fotoBase64);

                return View("RedefinirSenhaUsuario", modelRedefinirSenha);

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "ConfirmarCodigoSeguranca");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível validar seu código de recuperação de acesso.");
                return View("ConfirmarCodigoSeguranca", model);
            }

        }

        //
        // POST: /Usuario/RedefinirSenhaUsuario
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RedefinirSenhaUsuario(RedefinirSenhaUsuarioModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RedefinirSenhaUsuario", model);
            }

            try
            {

                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);

                UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.IdUsuario);
                if (_usuarioRegistrado == null || !_usuarioRegistrado.EmailUsuario.Equals(model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Desculpe tivemos problemas em identificar seu usuário registrado.");
                    return View("RedefinirSenhaUsuario");
                }

                //Alterar senha do usuário.
                negocioUsuario.RedefinirSenhaUsuario(model.IdUsuario, model.Senha, model.ConfirmaSenha);
                
                return RedirectToAction("Index", "Home");

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "RedefinirSenhaUsuario");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível redefinir sua senha de acesso.");
                return View("RedefinirSenhaUsuario", model);
            }

        }
        
        //
        // POST: /Usuario/SalvarFotoUsuario
        [HttpPost]
        public JsonResult JsonSalvarFotoUsuario()
        {
            string base64String = "iVBORw0KGgoAAAANSUhEUgAAAFsAAABbCAMAAAAr6AmrAAAAMFBMVEXFxcX+/v7CwsLIyMjNzc3V1dXh4eHm5ubs7Ozx8fH6+vrp6en09PT39/fc3NzS0tKCih/2AAACAElEQVRoge2Ya46DMAyEifOEpnD/2y6I3S1t42RiU6k/+A4wsgbHHjMMFxcXFxdfAO18Qtj5EGMM3p0sb5dk/pniYk9TdiGbZ3JwZwiTnUfzzjhbtTXk0lSQXp25aY0hVyp6JzlV5bTw0qsvGvFa1Vpxm6rSxsg9t7EhbUwQStNS7pAj9+VjZRsTRa4gZa9tLiscKXstXCJt6/33xygxxUPSxvh+aQqgduh/P/Q6VzmS4G0iXbIx9UsPoLQxl/bXa6N9kgX9jT35dT8ItNF3OXdLDwM0YoVD9pNzEJzfso3p7oB0FgZD5GsKFz0BhWdp+qG5qT3Lg1XLFWn02bC3qrQuJrtaIky644Eq4SrqL4e5vJTzPGilieZyJ2avvAXJ+pEbWVP2KlN8fV4lQab6pdGB+/eUCK92IMlqktyZFt073aVT9dE803ln9kh3irfuyndxXLp5V76J4x8U25RH0KsH7pAjAesWj6a1IxPyRMmhx8gzyMKn9kMv086FhJ5+7ywNcbIyRzbGViO2YwNPI1BYJKdx3Kt9SP2v5kisFQ7FS55aH8K3Akfltu+eUa8k1nHyOkvWr+m5wqEfVHW4kdW7EUqwW0JtyWYKo63tkg0mkZ9gN5soTrCbNXw5QZo7k+WT+0j5Y2rG64Pyr4MzPiXXKNphsnMrSSu22ZH8aMIfkDkUmuoPguQAAAAASUVORK5CYII=";
            string mensagem;
            try
            {
                if (IsFotoValida(out mensagem))
                {
                    HttpPostedFileBase file = Request.Files[0];
                    int idUsuario = UsuarioLogadoConfig.Instance.UsuarioLogado.IdUsuario;
                    InfoImagemFoto infoFoto = JsonConvert.DeserializeObject<InfoImagemFoto>(Request.Params["avatar_data"]);
                    byte[] binaryData = CropImagem(file.InputStream, (int)infoFoto.Width, (int)infoFoto.Height, (int)infoFoto.X, (int)infoFoto.Y);
                    base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
                    UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);
                    negocioUsuario.GravarFotoUsuario(idUsuario, binaryData);
                    
                    //Atualizar dados do usuário logado.
                    UsuarioLogadoConfig.Instance.AtualizarFotoUsuario(base64String);

                    return JsonResultSucesso("data:image/png;base64," + base64String, "Foto carregada com sucesso.");
                }
                else
                {
                    return JsonResultErro(mensagem);
                }
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonSalvarFotoUsuario()"));
            }
        }

        private bool IsFotoValida(out string mensagem)
        {
            const int sizeMaxFile = 2097152; //bytes = 2MB (MegaBytes);
            string[] contentTypeValid = { "image/png", "image/x-png", "image/gif", "image/jpeg" };
            const string mensagemErro = "Favor carregar uma imagem no formato (jpg, png ou gif) com no máximo de 2MB (MegaBytes).";

            if (Request.Files.Count == 0)
            {
                mensagem = mensagemErro;
                return false;
            }
            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength > sizeMaxFile)
            {
                mensagem = mensagemErro;
                return false;
            }
            string contentType = file.ContentType;
            if (!contentTypeValid.Contains<string>(contentType))
            {
                mensagem = mensagemErro;
                return false;
            }

            mensagem = "Arquivo validado com sucesso.";
            return true;

        }


        /// <summary>
        /// Realizar um corte na imagem informada e retornando os bytes da imagem recortada.
        /// </summary>
        /// <param name="streamImagem">Stream da imagem original a ser recortada.</param>
        /// <param name="width">Largura do corte da imagem.</param>
        /// <param name="height">Altura do corte da imagem.</param>
        /// <param name="x">Posição X do corte da imagem.</param>
        /// <param name="y">Posição Y do corte da imagem.</param>
        /// <returns></returns>
        private byte[] CropImagem(Stream streamImagem, int width, int height, int x, int y)
        {
            try
            {
                using (System.Drawing.Image imageOriginal = System.Drawing.Image.FromStream(streamImagem))
                {
                    using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, imageOriginal.PixelFormat))
                    {
                        bmp.SetResolution(imageOriginal.HorizontalResolution, imageOriginal.VerticalResolution);
                        using (System.Drawing.Graphics Graphic = System.Drawing.Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(imageOriginal, new System.Drawing.Rectangle(0, 0, width, height), x, y, width, height, System.Drawing.GraphicsUnit.Pixel);
                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, imageOriginal.RawFormat);
                            return ms.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                throw (erro);
            }
        }

        //
        //GET: //Usuario/AlterarSenhaUsuarioLogado
        [HttpGet]
        public ActionResult AlterarSenhaUsuarioLogado()
        {

            try
            {

                GFin.Negocio.Listeners.IUsuarioLogado _usuarioLogado = UsuarioLogadoConfig.Instance.UsuarioLogado;
                if (!_usuarioLogado.IsLogado)
                {
                    return RedirectToAction("Index", "Home");
                }

                //Montar model para solicitar redefinição de senha do usuário.
                AlterarSenhaUsuarioModel modelAlterarSenha = new AlterarSenhaUsuarioModel();
                modelAlterarSenha.IdUsuario = _usuarioLogado.IdUsuario;
                modelAlterarSenha.Nome = _usuarioLogado.NomeUsuario;
                modelAlterarSenha.Email = _usuarioLogado.EmailUsuario;
                modelAlterarSenha.FotoBase64 = _usuarioLogado.FotoBase64;

                return View("AlterarSenhaUsuarioLogado", modelAlterarSenha);

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "AlterarSenhaUsuarioLogado");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível abrir tela de redefinição de senha.");
                return View("Index", "Home");
            }

        }

        //
        // POST: /Usuario/AlterarSenhaUsuario
        [HttpPost]
        public ActionResult AlterarSenhaUsuario(AlterarSenhaUsuarioModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AlterarSenhaUsuarioLogado", model);
            }

            try
            {

                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);

                UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.IdUsuario);
                if (_usuarioRegistrado == null || !_usuarioRegistrado.EmailUsuario.Equals(model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Desculpe tivemos problemas em identificar seu usuário registrado.");
                    return View("AlterarSenhaUsuarioLogado", model);
                }

                //Alterar senha do usuário.
                negocioUsuario.RedefinirSenhaUsuario(model.IdUsuario, model.Senha, model.ConfirmaSenha);

                AlertaUsuario("Senha do usuário alterada com sucesso.", TipoAlertaEnum.Informacao);
                return View("AlterarSenhaUsuarioLogado", model);

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "AlterarSenhaUsuario");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível alterar a senha do usuário.");
                return View("AlterarSenhaUsuarioLogado", model);
            }

        }

        //
        //GET: //Usuario/AlterarDadosUsuarioLogado
        [HttpGet]
        public ActionResult AlterarDadosUsuarioLogado()
        {

            try
            {

                GFin.Negocio.Listeners.IUsuarioLogado _usuarioLogado = UsuarioLogadoConfig.Instance.UsuarioLogado;
                if (!_usuarioLogado.IsLogado)
                {
                    return RedirectToAction("Index", "Home");
                }

                UsuarioNegocio negocio = new UsuarioNegocio(UsuarioLogadoConfig.Instance);
                UsuarioAcessoEntidadeControle usuarioEntidade = negocio.ObterUsuarioAcessoEntidade(_usuarioLogado.IdUsuario);
                if (usuarioEntidade == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                AlterarDadosUsuarioModel modelAlterarDadosUsuario = new AlterarDadosUsuarioModel();

                //Dados do usuário
                modelAlterarDadosUsuario.IdUsuario = usuarioEntidade.UsuarioAcesso.Id;
                modelAlterarDadosUsuario.Nome = usuarioEntidade.UsuarioAcesso.NomeUsuario;
                modelAlterarDadosUsuario.Email = usuarioEntidade.UsuarioAcesso.EmailUsuario;
                string _fotoBase64 = "iVBORw0KGgoAAAANSUhEUgAAAFsAAABbCAMAAAAr6AmrAAAAMFBMVEXFxcX+/v7CwsLIyMjNzc3V1dXh4eHm5ubs7Ozx8fH6+vrp6en09PT39/fc3NzS0tKCih/2AAACAElEQVRoge2Ya46DMAyEifOEpnD/2y6I3S1t42RiU6k/+A4wsgbHHjMMFxcXFxdfAO18Qtj5EGMM3p0sb5dk/pniYk9TdiGbZ3JwZwiTnUfzzjhbtTXk0lSQXp25aY0hVyp6JzlV5bTw0qsvGvFa1Vpxm6rSxsg9t7EhbUwQStNS7pAj9+VjZRsTRa4gZa9tLiscKXstXCJt6/33xygxxUPSxvh+aQqgduh/P/Q6VzmS4G0iXbIx9UsPoLQxl/bXa6N9kgX9jT35dT8ItNF3OXdLDwM0YoVD9pNzEJzfso3p7oB0FgZD5GsKFz0BhWdp+qG5qT3Lg1XLFWn02bC3qrQuJrtaIky644Eq4SrqL4e5vJTzPGilieZyJ2avvAXJ+pEbWVP2KlN8fV4lQab6pdGB+/eUCK92IMlqktyZFt073aVT9dE803ln9kh3irfuyndxXLp5V76J4x8U25RH0KsH7pAjAesWj6a1IxPyRMmhx8gzyMKn9kMv086FhJ5+7ywNcbIyRzbGViO2YwNPI1BYJKdx3Kt9SP2v5kisFQ7FS55aH8K3Akfltu+eUa8k1nHyOkvWr+m5wqEfVHW4kdW7EUqwW0JtyWYKo63tkg0mkZ9gN5soTrCbNXw5QZo7k+WT+0j5Y2rG64Pyr4MzPiXXKNphsnMrSSu22ZH8aMIfkDkUmuoPguQAAAAASUVORK5CYII=";
                if (usuarioEntidade.UsuarioAcesso.FotoUsuario != null)
                    _fotoBase64 = System.Convert.ToBase64String(usuarioEntidade.UsuarioAcesso.FotoUsuario, 0, usuarioEntidade.UsuarioAcesso.FotoUsuario.Length);
                modelAlterarDadosUsuario.FotoBase64 = string.Format("data:image/png;base64,{0}", _fotoBase64);

                //Dados da entidade de controle.
                modelAlterarDadosUsuario.CodigoTipoEntidadeControle = usuarioEntidade.EntidadeControle.CodigoTipoEntidade;
                modelAlterarDadosUsuario.IdEntidade = usuarioEntidade.EntidadeControle.Id;
                modelAlterarDadosUsuario.NomeEntidadeControle = usuarioEntidade.EntidadeControle.NomeEntidade;
                modelAlterarDadosUsuario.CpfCnpjEntidadeControle = usuarioEntidade.EntidadeControle.CpfCnpjEntidade;

                return View("AlterarDadosUsuario", modelAlterarDadosUsuario);

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "AlterarDadosUsuarioLogado");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível abrir tela de alteração dos dados do usuário.");
                return View("Index", "Home");
            }

        }

        //
        // POST: /Usuario/AlterarDadosUsuario
        [HttpPost]
        public ActionResult AlterarDadosUsuario(AlterarDadosUsuarioModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("AlterarDadosUsuario", model);
            }

            try
            {

                UsuarioNegocio negocioUsuario = new UsuarioNegocio(UsuarioLogadoConfig.Instance);

                UsuarioSistema _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.IdUsuario);
                if (_usuarioRegistrado == null || !_usuarioRegistrado.EmailUsuario.Equals(model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Desculpe tivemos problemas em identificar seu usuário registrado.");
                    return View("AlterarDadosUsuario", model);
                }

                //Alterar dados do usuário.
                negocioUsuario.AlterarNomeEmailUsuario(model.IdUsuario, model.Nome, model.Email);

                EntidadeControleNegocio negocioEntidadeControle = new EntidadeControleNegocio(UsuarioLogadoConfig.Instance);
                
                //Alterar os dados da entidade controle.
                negocioEntidadeControle.AlterarNomeCnpjEntidadeControle(model.IdEntidade, model.CodigoTipoEntidadeControle, model.NomeEntidadeControle, model.CpfCnpjEntidadeControle);

                //Atualizar dados do usuário logado...
                UsuarioLogadoConfig.Instance.AtualizarDadosUsuario(model.Nome, model.Email, model.NomeEntidadeControle);

                AlertaUsuario("Seus dados cadastrais foram alterados com sucesso.", TipoAlertaEnum.Informacao);
                return View("AlterarDadosUsuario", model);

            }
            catch (Exception erro)
            {
                AppLogger.Log.Get().LogError(erro, "AlterarDadosUsuario");
                ModelState.AddModelError(string.Empty, "Desculpe, não foi possível alterar seu dados cadastrais.");
                return View("AlterarDadosUsuario", model);
            }

        }
    }
}
