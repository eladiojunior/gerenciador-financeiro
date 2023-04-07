using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using gfin.webapi.Api.Models;
using gfin.webapi.Api.Models.Usuario;
using gfin.webapi.Dados;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio;
using gfin.webapi.Negocio.Listeners;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace gfin.webapi.Controllers
{
    public class UsuarioController : GenericController
    {
        private readonly ILogger<NaturezaContaController> _logger;
        private readonly GFinContext _context;
        private readonly IClienteListener _cliente;
        private readonly IConfiguration _configuration;

        public UsuarioController(ILogger<NaturezaContaController> logger, GFinContext context, IClienteListener cliente, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _cliente = cliente;
            _configuration = configuration;
        }

        /// <summary>
        /// Gera um JWT do usuário logado para utilização no processo de consumo das APIs.
        /// </summary>
        /// <param name="usuario">Informações do usuário.</param>
        /// <returns></returns>
        private string ObterJSONWebToken(UsuarioSistema usuario)  
        {  
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));  
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);  
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],  
                _configuration["Jwt:Issuer"],  
                null,
                expires: DateTime.Now.AddMinutes(120),  
                signingCredentials: credentials);  
            return new JwtSecurityTokenHandler().WriteToken(token);  
        }
        
        [AllowAnonymous]  
        [HttpPost(nameof(Login))]  
        public async Task<ActionResult<ResultBase>> Login([FromBody] LoginModel model)  
        {  
            var result = new ResultBase();
            result.Status = false;
            if (model == null) 
            {
                result.Mensagem = "Informações de login e senha não informado.";
                return Unauthorized(result);
            }
            
            var negocioUsuario = new UsuarioNegocio(_cliente, _context, _configuration);

            //Verificar se o usuário já está registrado, mas não confirmou o registro.
            var _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.Email);
            if (_usuarioRegistrado != null && _usuarioRegistrado.IsConfirmacaoEmailUsuario == false)
            {//Usuário registrado e sem confirmação de registro.
                result.Mensagem = "Usuário ainda não confirmou seu registro, caso não tenha recebido um e-mail para confirmação, solicite o reenvio.";
                return Ok(result);
            }
            
            //Realizar o login do usuário.
            _usuarioRegistrado = negocioUsuario.Autenticar(model.Email, model.Senha);
            if (_usuarioRegistrado == null)
            {
                result.Mensagem = "E-mail e/ou senha informados é inválido.";
                return Unauthorized(result);
            }
            
            //Verificar a entidade que está vinculada ao Usuário
            var _usuarioAcessoEntidade = negocioUsuario.ObterUsuarioAcessoEntidade(model.IdEntidade, _usuarioRegistrado.Id);
            if (_usuarioAcessoEntidade == null)
            {
                result.Mensagem = "Não foi possível identificar a entidade de controle do usuário.";
                return Unauthorized(result);
            }

            //Gerar o JWT do usuário autenticado...
            var tokenString = ObterJSONWebToken(_usuarioRegistrado);  
            result.Status = true;
            result.Mensagem = "Usuário autenticado com sucesso.";
            result.Data = tokenString;
            
            return Ok(result);  
        } 
        
        /// <summary>  
        /// Authorize the Method  
        /// </summary>  
        /// <returns></returns>  
        [HttpGet(nameof(Get))]  
        public async Task<IEnumerable<string>> Get()  
        {  
            var accessToken = await HttpContext.GetTokenAsync("access_token");  
  
            return new string[] { accessToken };  
        }

        [AllowAnonymous]
        [HttpPost(nameof(Registrar))]
        public async Task<ActionResult<ResultBase>> Registrar([FromBody] UsuarioRegistroModel model)
        {
            var result = new ResultBase();
            result.Status = false;
            if (model == null) 
            {
                result.Mensagem = "Informações de registro do usuário não informado.";
                return Ok(result);
            }

            try
            {

                var negocioUsuario = new UsuarioNegocio(_cliente, _context, _configuration);
            
                //Verificar se o usuário já está registrado, mas não confirmou o registro.
                var _usuarioRegistrado = negocioUsuario.ObterUsuarioSistema(model.Email);
                if (_usuarioRegistrado is { IsConfirmacaoEmailUsuario: false })
                {//Usuário registrado e sem confirmação de registro.
                    result.Mensagem = $"Usuário {_usuarioRegistrado.EmailUsuario} já registrado e ativo.";
                    return Ok(result);
                }

                //Criar objeto para inclusão.
                var usuarioAcessoEntidade = CarregarObjetoAcessoEntidade(model);
                negocioUsuario.RegistrarUsuario(usuarioAcessoEntidade);

                result.Status = true;
                result.Mensagem = $"Usuário registrado com sucesso. Foi enviado uma notificação para o e-mail do usuário, realize a validação do registro pelo link enviado no e-mail.";
                return Ok(result);

            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
                result.Mensagem = "Erro ao registrar novo usuário.";
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Criar objeto com as informações necessárias para registro do usuário.
        /// </summary>
        /// <param name="model">Informações enviadas pelo usuário.</param>
        /// <returns></returns>
        private UsuarioAcessoEntidadeControle CarregarObjetoAcessoEntidade(UsuarioRegistroModel model)
        {
            
            var usuarioAcessoEntidade = new UsuarioAcessoEntidadeControle();
            usuarioAcessoEntidade.CodigoTipoPerfilAcesso = (short)Dados.Enums.TipoPerfilAcessoUsuarioEnum.Responsavel;
            usuarioAcessoEntidade.DataFinallVigenciaAcessoUsuario = null;
            usuarioAcessoEntidade.DataInicialVigenciaAcessoUsuario = DateTime.Now;
            usuarioAcessoEntidade.DataRegistroUsuarioAcesso = DateTime.Now;
            
            var entidade = new EntidadeControle();
            entidade.CodigoTipoEntidade = model.CodigoTipoEntidadeControle;
            entidade.NomeEntidade = (model.CodigoTipoEntidadeControle == (short)Dados.Enums.TipoEntidadeControleEnum.Fisica ? "Minha Casa" : model.NomeEntidadeControle);
            entidade.CpfCnpjEntidade = model.CpfCnpjEntidadeControle;
            entidade.CodigoTipoSituacaoEntidade = (short)Dados.Enums.TipoSituacaoEnum.Ativo;
            usuarioAcessoEntidade.EntidadeControle = entidade;

            var usuarioAcesso = new UsuarioSistema();
            usuarioAcesso.NomeUsuario = model.Nome;
            usuarioAcesso.SenhaUsuario = model.Senha;
            usuarioAcesso.EmailUsuario = model.Email;
            usuarioAcesso.DataHoraRegistroUsuario = DateTime.Now;
            usuarioAcesso.CodigoTipoSituacaoUsuario = (short)Dados.Enums.TipoSituacaoEnum.Ativo;
            usuarioAcessoEntidade.UsuarioAcesso = usuarioAcesso;
            return usuarioAcessoEntidade;
        }
        
    }
}