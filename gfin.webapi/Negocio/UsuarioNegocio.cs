using gfin.webapi.Dados;
using gfin.webapi.Dados.Enums;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio.Erros;
using gfin.webapi.Negocio.Listeners;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace gfin.webapi.Negocio
{
    public class UsuarioNegocio : GenericNegocio
    {
        private readonly IConfiguration _configuration;
        public UsuarioNegocio(IClienteListener cliente, GFinContext context, IConfiguration configuration) : base(cliente, context)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Registra uma usuario no sistema;
        /// </summary>
        /// <param name="usuario">Informações da usuario a ser registrada.</param>
        public UsuarioAcessoEntidadeControle RegistrarUsuario(UsuarioAcessoEntidadeControle usuarioAcessoEntidade)
        {
            
            //Inclusão de apenas uma usuario. 
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {

                ValidarEntidadeControle(usuarioAcessoEntidade.EntidadeControle);
                usuarioAcessoEntidade.EntidadeControle.CodigoTipoSituacaoEntidade = (short)TipoSituacaoEnum.Ativo;
                usuarioAcessoEntidade.EntidadeControle.DataHoraRegistro = DateTime.Now;
            
                ValidarUsuario(usuarioAcessoEntidade.UsuarioAcesso);
                usuarioAcessoEntidade.UsuarioAcesso.CodigoTipoSituacaoUsuario = (short)TipoSituacaoEnum.Ativo;
                usuarioAcessoEntidade.UsuarioAcesso.DataHoraRegistroUsuario = DateTime.Now;
                usuarioAcessoEntidade.UsuarioAcesso.IsAlterarSenhaUsuario = false; //Não pedir para mudar a senha.
                usuarioAcessoEntidade.UsuarioAcesso.IsConfirmacaoEmailUsuario = false; //Solicitar confirmação de e-mail.
            
                //Verificar se o usuário já possui uma entidade física (Minha Casa) como Responsável.
                int qtdRegs = uofw.UsuarioAcessoEntidadeControle.QuantRegistros(uaec =>
                    uaec.UsuarioAcesso.EmailUsuario.Equals(usuarioAcessoEntidade.UsuarioAcesso.EmailUsuario) && 
                    uaec.CodigoTipoPerfilAcesso == (short)TipoPerfilAcessoUsuarioEnum.Responsavel &&
                    uaec.EntidadeControle.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Fisica, uaec => uaec.EntidadeControle, uaec => uaec.UsuarioAcesso);
                if (qtdRegs > 0)
                {//Usuário já possui é responsável por uma entidade física (Minha Casa);
                    throw new NegocioException("Você já é responsável por uma entidade: Minha Casa, desculpe não será possível registra-lo.");
                }

                usuarioAcessoEntidade.EntidadeControle = uofw.EntidadeControle.Incluir(usuarioAcessoEntidade.EntidadeControle);

                usuarioAcessoEntidade.IdUsuarioAcesso = usuarioAcessoEntidade.UsuarioAcesso.Id;
                if (usuarioAcessoEntidade.UsuarioAcesso.Id == 0)
                {//Usuário ainda não registrado...
                    //Gerar salt de senha...
                    usuarioAcessoEntidade.UsuarioAcesso.SaltSenhaUsuario = UtilNegocio.GerarSaltPassword();
                    string _senha = usuarioAcessoEntidade.UsuarioAcesso.SenhaUsuario;
                    //Incluir novo usuário...
                    usuarioAcessoEntidade.UsuarioAcesso.SenhaUsuario = UtilNegocio.GerarHashPassword(_senha, usuarioAcessoEntidade.UsuarioAcesso.SaltSenhaUsuario);
                    usuarioAcessoEntidade.UsuarioAcesso = uofw.UsuarioSistema.Incluir(usuarioAcessoEntidade.UsuarioAcesso);
                }
                
                usuarioAcessoEntidade = uofw.UsuarioAcessoEntidadeControle.Incluir(usuarioAcessoEntidade);
                uofw.SalvarAlteracoes();

            }
            
            //Registrar histórico de registro de usuário no sistema...
            GravarHistoricoUsuario(usuarioAcessoEntidade.UsuarioAcesso.Id,
                    TipoOperacaoHistoricoUsuarioEnum.RegistrarUsuario,
                    $"Registrado com o perfil [{UtilEnum.GetTextoTipoPerfilAcessoUsuario(usuarioAcessoEntidade.CodigoTipoPerfilAcesso)}]");

            //Enviar e-mail de confirmação de registro.
            EnviarSolicitacaoConfirmacaoEmailUsuario(usuarioAcessoEntidade.UsuarioAcesso);

            return usuarioAcessoEntidade;

        }

        /// <summary>
        /// Realiza a validação das informações da entidade de controle.
        /// </summary>
        /// <param name="entidadeControle">Entidade de controle a ser validada.</param>
        private void ValidarEntidadeControle(EntidadeControle entidadeControle)
        {
            if (entidadeControle == null)
                throw new NegocioException("Nenhuma informação da entidade de controle preenchida.");
            if (entidadeControle.CodigoTipoEntidade == 0)
                throw new NegocioException("Tipo da entidade de controle não informada.");
            if (entidadeControle.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Juridica)
            {
                if (string.IsNullOrEmpty(entidadeControle.CpfCnpjEntidade))
                    throw new NegocioException("Número do CNPJ da empresa não informada.");
                if (!UtilNegocio.ValidarCNPJ(entidadeControle.CpfCnpjEntidade))
                    throw new NegocioException("Número do CNPJ da empresa inválido.");
                if (string.IsNullOrEmpty(entidadeControle.NomeEntidade))
                    throw new NegocioException("Nome da empresa não informada.");
            }
            if (entidadeControle.CodigoTipoEntidade == (short)TipoEntidadeControleEnum.Fisica)
            {
                if (!string.IsNullOrEmpty(entidadeControle.CpfCnpjEntidade) && 
                   (!UtilNegocio.ValidarCPF(entidadeControle.CpfCnpjEntidade)))
                    throw new NegocioException("Número do CPF da entidade de controle inválido.");
            }
        }

        /// <summary>
        /// Envia e-mail para que o usuário possa confirmar seu e-mail e finalizar o seu registro no site.
        /// </summary>
        /// <param name="usuarioSistema">Informações do usuário para envio do e-mail.</param>
        public void EnviarSolicitacaoConfirmacaoEmailUsuario(UsuarioSistema usuarioSistema)
        {

            //Recuperar mensagem para o envio.
            var pathEmailConfirmacao =
                $"{ClienteListener.MapPathAppServidor}\\Content\\html\\emailConfirmacaoUsuario.html";
            var mensagemEmail = File.ReadAllText(pathEmailConfirmacao);

            //Alterar parâmetros da mensagem.
            mensagemEmail = mensagemEmail.Replace("#DATA_ENVIO_EMAIL#", DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"));
            mensagemEmail = mensagemEmail.Replace("#NOME#", usuarioSistema.NomeUsuario);
            string linkConfirmacaoEmail = _configuration.GetValue<String>("linkConfirmacaoUsuario");
            linkConfirmacaoEmail = string.Format(linkConfirmacaoEmail, UtilNegocio.Criptografar(usuarioSistema.Id.ToString()));
            mensagemEmail = mensagemEmail.Replace("#LINK_CONFIRMCAO_EMAIL#", linkConfirmacaoEmail);

            string cidAnexo = "CID_IMG_CABECALHO_EMAIL";
            mensagemEmail = mensagemEmail.Replace("#IMG_CABECALHO_EMAIL#", cidAnexo);

            //Anexar imagem 'embedder'...
            List<EmailAnexo> anexos = null;
            var imgAnexo = UtilNegocio.ObterAnexoImagemCabecalho(cidAnexo, ClienteListener.MapPathAppServidor);
            if (imgAnexo != null)
            {
                anexos = new List<EmailAnexo>();
                anexos.Add(imgAnexo);
            }

            EmailNegocio.Instance(_configuration).EnviarEmail(usuarioSistema.EmailUsuario, usuarioSistema.NomeUsuario, "Gerenciador Financeiro - Confirmação de Registro", mensagemEmail, anexos);
 
        }

        /// <summary>
        /// Realiza a validação das informações da usuario antes de seu registro ou alteração.
        /// </summary>
        /// <param name="usuario">Informações da usuario a ser validada.</param>
        /// <returns></returns>
        private void ValidarUsuario(UsuarioSistema usuario)
        {
            if (usuario == null)
                throw new NegocioException("Nenhuma informação do usuário preenchida.");

            if (String.IsNullOrEmpty(usuario.NomeUsuario))
                throw new NegocioException("Nome do usuario não informado.");
            
            if (String.IsNullOrEmpty(usuario.EmailUsuario))
                throw new NegocioException("E-mail do usuario não informado.");
            
            if (!UtilNegocio.ValidarEmail(usuario.EmailUsuario))
                throw new NegocioException(
                    $"E-mail do usuário [{usuario.EmailUsuario}] é inválido, favor informar um e-mail válido.");
            
            if (String.IsNullOrEmpty(usuario.SenhaUsuario))
                throw new NegocioException("Senha do usuario não informada.");
            

            //Verificar a existência do e-mail de usuário.
            if (usuario.Id == 0)
            {
                
                if (!UtilNegocio.ValidarSenha(usuario.SenhaUsuario))
                    throw new NegocioException("Senha do usuario inválida, informar no mínimo 6 caracteres (letras, números e caracteres especiais).");

                if (IsUsuarioSistemaExistente(usuario.EmailUsuario))
                    throw new NegocioException(
                        $"E-mail do usuário [{usuario.EmailUsuario}] já registrado, caso tenha esquecido sua senha, utilize a funcionalidade de 'esqueci minha senha'.");

            }

        }

        /// <summary>
        /// Recupera um usuário do sistema pelo seu e-mail (login).
        /// </summary>
        /// <param name="email">E-mail de login do usuário que será recuperado.</param>
        /// <returns></returns>
        public UsuarioSistema ObterUsuarioSistema(string email)
        {
            UsuarioSistema result = null;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                result = uofw.UsuarioSistema.Obter(us => us.EmailUsuario.Equals(email) && us.CodigoTipoSituacaoUsuario == (short)TipoSituacaoEnum.Ativo);
            }
            return result;
        }

        /// <summary>
        /// Verifica se o usuário do sistema existe nos registros, pelo seu e-mail.
        /// </summary>
        /// <param name="email">E-mail do usário a ser verificado.</param>
        /// <returns></returns>
        private bool IsUsuarioSistemaExistente(string email)
        {
            bool isExiste = false;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                int qtdRegs = uofw.UsuarioSistema.QuantRegistros(us => us.EmailUsuario.Equals(email));
                isExiste = (qtdRegs > 0);
            }
            return isExiste;
        }

        /// <summary>
        /// Recupera uma usuario pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da usuario.</param>
        /// <returns></returns>
        public UsuarioSistema ObterUsuarioSistema(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.UsuarioSistema.ObterPorId(id);
            }
        }

        /// <summary>
        /// Realizar a alteração das informações da usuario, conforme as informações enviadas.
        /// </summary>
        /// <param name="usuario">Informações da usuario a ser alterada.</param>
        public void GravarUsuarioSistema(UsuarioSistema usuario)
        {
            ValidarUsuario(usuario);
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                uofw.UsuarioSistema.Alterar(usuario);
                uofw.SalvarAlteracoes();
            }
        }

        /// <summary>
        /// Realiza autenticação do usuário, pelo seu e-mail e senha.
        /// </summary>
        /// <param name="email">E-mail do usuário a ser autenticado.</param>
        /// <param name="senha">Senha do usuário a ser autenticado.</param>
        /// <returns></returns>
        public UsuarioSistema Autenticar(string email, string senha)
        {
            UsuarioSistema _usuario = null;
            DateTime dataUltimoAcesso = DateTime.Now;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                _usuario = uofw.UsuarioSistema.Obter(us => us.EmailUsuario.Equals(email) && us.CodigoTipoSituacaoUsuario == (short)TipoSituacaoEnum.Ativo);
                if (_usuario != null)
                {
                    if (_usuario.DataUltimoAcessoUsuario.HasValue)
                    {
                        dataUltimoAcesso = _usuario.DataUltimoAcessoUsuario.Value;
                    }
                    string _senhaCriptografada = UtilNegocio.GerarHashPassword(senha, _usuario.SaltSenhaUsuario);
                    if (!_senhaCriptografada.Equals(_usuario.SenhaUsuario))
                    {//Senha não confere...
                        _usuario = null;
                    }
                    else
                    {//Senha confere... registrar log de autenticação...
                        //Atualizar data último acesso do usuário...
                        _usuario.DataUltimoAcessoUsuario = DateTime.Now;
                        uofw.UsuarioSistema.Alterar(_usuario);
                        uofw.SalvarAlteracoes();
                    }
                }
                //Retornar a data e hora do último acesso do usuário...
                if (_usuario != null)
                {
                    _usuario.DataUltimoAcessoUsuario = dataUltimoAcesso;
                    //Registrar histórico de autenticação...
                    GravarHistoricoUsuario(_usuario.Id,
                            TipoOperacaoHistoricoUsuarioEnum.AutenticarUsuario,
                            "Autenticação do usuário por login e senha.");
                }
            }
            return _usuario;
        }

        /// <summary>
        /// Obter um usuário acesso entidade controle pelo identificador.
        /// </summary>
        /// <param name="idUsuarioAcesso">Identificador do Usuário Acesso Entidade [UsuarioSistema]&lt;==&gt;[EntidadeControle];</param>
        /// <returns></returns>
        public UsuarioAcessoEntidadeControle ObterUsuarioAcessoEntidade(int idUsuarioAcesso, bool isReferencia = true)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                if (isReferencia)
                    return uofw.UsuarioAcessoEntidadeControle.Obter(uaec => uaec.Id == idUsuarioAcesso, uaec => uaec.UsuarioAcesso, uaec => uaec.EntidadeControle);
                else
                    return uofw.UsuarioAcessoEntidadeControle.ObterPorId(idUsuarioAcesso);
            }
        }

        /// <summary>
        /// Realizar a confirmação de e-mail do usuário registrado.
        /// Altera o indicador de confirmação de e-mail do usuário no sistema e registra log desta ação.
        /// </summary>
        /// <param name="usuarioSistema">Informações do usuário para confirmação do e-mail do usuário.</param>
        public void ConfirmarEmailUsuario(UsuarioSistema usuarioSistema)
        {
            if (usuarioSistema == null)
                throw new NegocioException("Nenhuma informação de usuário preenchida.");

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                
                usuarioSistema.IsConfirmacaoEmailUsuario = true;
                uofw.UsuarioSistema.Alterar(usuarioSistema);
                uofw.SalvarAlteracoes();

            }
            
            //Registrar histórico de confirmação de e-mail de usuário...
            GravarHistoricoUsuario(usuarioSistema.Id,
                TipoOperacaoHistoricoUsuarioEnum.ConfirmarEmailUsuario,
                $"Confirmado em {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}.");

        }

        /// <summary>
        /// Recupera um usuário acesso entidade controle pelo id do usuário e id da entidade.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="idUsuario">Identificador do usuário.</param>
        /// <param name="isLoadRefUsuario">Indicador para recuperar a referencia do vinculo com o usuário.</param>
        /// <returns></returns>
        public UsuarioAcessoEntidadeControle ObterUsuarioAcessoEntidade(int idEntidade, int idUsuario, bool isLoadRefUsuario = false)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                if (isLoadRefUsuario)
                    return uofw.UsuarioAcessoEntidadeControle.Obter(uaec => uaec.IdEntidade == idEntidade && 
                        uaec.IdUsuarioAcesso == idUsuario, uaec => uaec.EntidadeControle, uaec => uaec.UsuarioAcesso);
                //Não carregar as referências do Usuário, somente da Entidade.
                return uofw.UsuarioAcessoEntidadeControle.Obter(uaec => uaec.IdEntidade == idEntidade &&
                    uaec.IdUsuarioAcesso == idUsuario, uaec => uaec.EntidadeControle);
            }
        }

        /// <summary>
        /// Envia e-mail de solicitação de recuperação de acesso do usuário.
        /// </summary>
        /// <param name="nomeUsuario">Nome do usuário para ser colocado no e-mail enviado para o usuário.</param>
        /// <param name="emailUsuario">E-mail de destino da solicitação.</param>
        /// <param name="codigoSeguranca">Código de segurança enviado para o usuário recuperar o acesso.</param>
        public void EnviarSolicitacaoRecuperacaoSenhaUsuario(string nomeUsuario, string emailUsuario, string codigoSeguranca)
        {
            
            //Recuperar mensagem para o envio.
            var pathHtmlEmail =
                $"{ClienteListener.MapPathAppServidor}\\Content\\html\\emailRecuperacaoSenhaUsuario.html";
            var mensagemEmail = File.ReadAllText(pathHtmlEmail);

            //Alterar parâmetros da mensagem.
            mensagemEmail = mensagemEmail.Replace("#DATA_ENVIO_EMAIL#", DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"));
            mensagemEmail = mensagemEmail.Replace("#NOME#", nomeUsuario);
            mensagemEmail = mensagemEmail.Replace("#CODIGO_SEGURANCA#", codigoSeguranca);
            string cidAnexo = "CID_IMG_CABECALHO_EMAIL";
            mensagemEmail = mensagemEmail.Replace("#IMG_CABECALHO_EMAIL#", cidAnexo);

            //Anexar imagem 'embedder'...
            List<EmailAnexo> anexos = null;
            var imgAnexo = UtilNegocio.ObterAnexoImagemCabecalho(cidAnexo, ClienteListener.MapPathAppServidor);
            if (imgAnexo != null)
            {
                anexos = new List<EmailAnexo>();
                anexos.Add(imgAnexo);
            }

            EmailNegocio.Instance(_configuration).EnviarEmail(emailUsuario, nomeUsuario, "Gerenciador Financeiro - Recuperação de Senha", mensagemEmail, anexos);

        }

        /// <summary>
        /// Valida o código de recuperação de acesso enviado para o usuário quando
        /// este está com problemas em acessar a aplicação.
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário para validação do código de recuperação.</param>
        /// <param name="emailUsuario">E-mail do usuário para confirmação de sua autenticidade.</param>
        /// <param name="codigoSegurancaUsuario">Código de recuperação a ser validado.</param>
        /// <returns></returns>
        public bool ValidarCodigoRecuperacaoAcessoUsuario(int idUsuario, string emailUsuario, string codigoSegurancaUsuario)
        {
            bool isValido = false;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                HistoricoUsuario ultimoHistorico = uofw.HistoricoUsuario.ObterUltimoHistoricoUsuario(idUsuario, TipoOperacaoHistoricoUsuarioEnum.RecuperarSenhaAcesso);
                if (ultimoHistorico != null && ultimoHistorico.Usuario != null)
                {
                    isValido = (ultimoHistorico.Usuario.EmailUsuario.Equals(emailUsuario) && 
                        ultimoHistorico.TextoHistoricoUsuario.Trim().Equals(codigoSegurancaUsuario));
                }
            }
            return isValido;            
        }

        /// <summary>
        /// Redefinição de senha do usuário, conforme seu Identificador.
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário para redefinição de senha.</param>
        /// <param name="novaSenha">Nova senha do usuário.</param>
        /// <param name="confirmacao">Confirmação da nova senha.</param>
        public void RedefinirSenhaUsuario(int idUsuario, string novaSenha, string confirmacao)
        {

            if (idUsuario == 0)
                throw new NegocioException("Identificador do usuário não informado.");

            if (String.IsNullOrEmpty(novaSenha))
                throw new NegocioException("Nova senha do usuario não informada.");
                
            if (!UtilNegocio.ValidarSenha(novaSenha))
                throw new NegocioException("Nova senha do usuario inválida, informar no mínimo 6 caracteres (letras, números e caracteres especiais).");

            UsuarioSistema _usuario = ObterUsuarioSistema(idUsuario);
            if (_usuario == null)
                throw new NegocioException(
                    $"Não foi possível encontrar o usário com o identificador [{idUsuario}] informado.");

            string antigaSenha = _usuario.SenhaUsuario;
            string antigoSaltSenha = _usuario.SaltSenhaUsuario;

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                //Gerar salt de senha e criptografar a nova senha...
                _usuario.SaltSenhaUsuario = UtilNegocio.GerarSaltPassword();
                _usuario.SenhaUsuario = UtilNegocio.GerarHashPassword(novaSenha, _usuario.SaltSenhaUsuario);
                //Setar para não solicitar a mudança de senha no próximo logon...
                _usuario.IsAlterarSenhaUsuario = false;

                uofw.UsuarioSistema.Alterar(_usuario);
                uofw.SalvarAlteracoes();

            }

            //Registrar histórico de alteração de senha do usuário...
            GravarHistoricoUsuario(_usuario.Id,
                TipoOperacaoHistoricoUsuarioEnum.AlterarSenhaUsuario,
                $"Senha antiga [{antigaSenha}], salt [{antigoSaltSenha}].");

        }

        /// <summary>
        /// Altera a foto do usuário no seu perfil, pelo seu identificador.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="bytesFoto"></param>
        public void GravarFotoUsuario(int idUsuario, byte[] bytesFoto)
        {

            if (idUsuario == 0)
                throw new NegocioException("Identificador do usuário não informado.");
            if (bytesFoto == null || bytesFoto.Length == 0)
                throw new NegocioException("Foto do usuário não informada.");

            UsuarioSistema _usuario = ObterUsuarioSistema(idUsuario);
            if (_usuario == null)
                throw new NegocioException(
                    $"Não foi possível encontrar o usário com o identificador [{idUsuario}] informado.");

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                _usuario.FotoUsuario = bytesFoto;
                uofw.UsuarioSistema.Alterar(_usuario);
                uofw.SalvarAlteracoes();
            }

            //Registrar histórico de alteração de foto do usuário...
            GravarHistoricoUsuario(_usuario.Id,
                TipoOperacaoHistoricoUsuarioEnum.MudarFotoUsuario, "Foto alterada.");
            
        }

        /// <summary>
        /// Realiza a alteração das informações básicas do usuário, Nome e E-mail.
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário se terá seus dados alterados.</param>
        /// <param name="nomeUsuario">Nome do usuário para alteração.</param>
        /// <param name="emailUsuario">E-mail do usuário para alteração dos dados.</param>
        public void AlterarNomeEmailUsuario(int idUsuario, string nomeUsuario, string emailUsuario)
        {
            if (idUsuario == 0)
                throw new NegocioException("Identificador do usuário não informado.");
            if (string.IsNullOrEmpty(nomeUsuario))
                throw new NegocioException("Nome do usuário não informado.");
            if (string.IsNullOrEmpty(emailUsuario))
                throw new NegocioException("E-mail do usuário, utilizado no login, não informado.");
            if (!UtilNegocio.ValidarEmail(emailUsuario))
                throw new NegocioException($"E-mail do usuário [{emailUsuario}], utilizado no login, inválido.");

            UsuarioSistema _usuario = ObterUsuarioSistema(idUsuario);
            if (_usuario == null)
                throw new NegocioException(
                    $"Não foi possível encontrar o usário com o identificador [{idUsuario}] informado.");
            
            if (!nomeUsuario.Equals(_usuario.NomeUsuario, StringComparison.CurrentCultureIgnoreCase) ||
                !emailUsuario.Equals(_usuario.EmailUsuario, StringComparison.CurrentCultureIgnoreCase)) 
            {//Somente alterar os dados caso identificado mudança.

                string _nomeAnterior = _usuario.NomeUsuario;
                string _emailAnterior = _usuario.EmailUsuario;
                using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
                {
                    _usuario.NomeUsuario = nomeUsuario;
                    _usuario.EmailUsuario = emailUsuario;
                    uofw.UsuarioSistema.Alterar(_usuario);
                    uofw.SalvarAlteracoes();
                }
                //Registrar histórico de alteração de dados cadastrais do usuário...
                GravarHistoricoUsuario(_usuario.Id,
                    TipoOperacaoHistoricoUsuarioEnum.AlterarDadosUsuario,
                    $"Dados anteriores - Nome: {_nomeAnterior}, e-mai: {_emailAnterior}");

            }
            
        }

    }

}
