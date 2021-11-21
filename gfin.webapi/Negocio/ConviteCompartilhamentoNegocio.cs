using gfin.webapi.Dados;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.IO;
using gfin.webapi.Negocio.Listeners;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace gfin.webapi.Negocio
{
    public class ConviteCompartilhamentoNegocio : GenericNegocio
    {
        private readonly IConfiguration _configuration;
        public ConviteCompartilhamentoNegocio(IClienteListener cliente, GFinContext context, IConfiguration configuration) : base(cliente, context)
        {
            _configuration = configuration;
        }
        
        /// <summary>
        /// Verifica se existem convidados para a entidade que está sendo acessada.
        /// </summary>
        /// <returns></returns>
        public bool HasConvidados()
        {
            bool result = false;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                int qtd = uofw.ConviteCompartilhamento.QuantRegistros(c => c.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade);
                result = (qtd != 0);
            }
            return result;
        }
        /// <summary>
        /// Recupera a lista de convidados registrados.
        /// </summary>
        /// <returns></returns>
        public List<ConviteCompartilhamento> ListarConvidados()
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.ConviteCompartilhamento.Listar(c => c.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade);
            }
        }

        /// <summary>
        /// Registrar convites de compartilhamento do controle financeiro.
        /// </summary>
        /// <param name="correntista">Informações do correntista.</param>
        public List<ConviteCompartilhamento> RegistrarConvites(List<ConviteCompartilhamento> listaConvites)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                foreach (var item in listaConvites)
                {
                    RegistrarConvites(uofw, item);
                }
                uofw.SalvarAlteracoes();

                //Gravar historico de envio de convites...
                string historico = "Você convitou {0} pessoa(s) para compartilhar seu controle financeiro, com a permissão {1}.{2}";
                historico = string.Format(historico, listaConvites.Count, Dados.Enums.UtilEnum.GetTextoTipoPermissaoCompartilhamento(listaConvites[0].CodigoPermissaoCompartilhamento), ListarEmailsParaHistorico(listaConvites));
                GravarHistoricoUsuario(ClienteListener.UsuarioLogado.IdUsuario, Dados.Enums.TipoOperacaoHistoricoUsuarioEnum.ConvidarUsuarioExterno, historico);

                //Enviar convites para as pessoas informadas...
                foreach (var item in listaConvites)
                {
                    EnviarConviteCompartilhamento(item);
                }
                return listaConvites;
            }
        }

        /// <summary>
        /// Recupera a lista de e-mails concatenados para armazenar no histórico.
        /// </summary>
        /// <param name="listaConvites">Lista de convites.</param>
        /// <returns></returns>
        private string ListarEmailsParaHistorico(List<ConviteCompartilhamento> listaConvites)
        {
            string listEmails = "";
            foreach (var item in listaConvites)
                listEmails += "\r\n" + item.EmailConvidado + ";";
            return listEmails;
        }

        /// <summary>
        /// Enviar e-mails para as pessoas convidadas.
        /// </summary>
        /// <param name="listaConvites">Lista de convites.</param>
        private void EnviarConviteCompartilhamento(ConviteCompartilhamento convite)
        {

            //Recuperar mensagem para o envio.
            var pathEmailConfirmacao =
                $"{ClienteListener.MapPathAppServidor}\\Content\\html\\emailCompartilharControle.html";
            var mensagemEmail = File.ReadAllText(pathEmailConfirmacao);
            var nomeUsuario = ClienteListener.UsuarioLogado.NomeUsuario;
            //Alterar parâmetros da mensagem.
            mensagemEmail = mensagemEmail.Replace("#DATA_ENVIO_EMAIL#", DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"));
            var nomeConvidado = string.IsNullOrEmpty(convite.NomeConvidado) ? "Amigo(a) do " + UtilNegocio.ObterNomeReduzido(nomeUsuario) : convite.NomeConvidado;
            mensagemEmail = mensagemEmail.Replace("#NOME#", convite.NomeConvidado);
            mensagemEmail = mensagemEmail.Replace("#NOME_COMPLETO#", nomeUsuario);
            var nomeEntidade = ClienteListener.UsuarioLogado.NomeEntidade;
            if (ClienteListener.UsuarioLogado.IsEmpresa)
                nomeEntidade = $"Empresa ({nomeEntidade})";
            mensagemEmail = mensagemEmail.Replace("#ENTIDADE#", nomeEntidade);
            string mensagemAmigoa = "";
            if (!string.IsNullOrEmpty(convite.MensagemConvite))
            {
                mensagemAmigoa =
                    $"<p style=\"padding: 5px; margin-bottom: 10px; border: 1px solid transparent; border-radius: 4px; color: #31708f; background-color: #d9edf7; border-color: #bce8f1;\">{convite.MensagemConvite.Replace("\r\n", "<br/>")}</p>";
            }
            mensagemEmail = mensagemEmail.Replace("#MENSAGEM_AMIGO_A#", mensagemAmigoa);

            string linkAceitarConviteCompartilhamento = _configuration.GetValue<String>("linkAceitarConviteCompartilhamento");
            linkAceitarConviteCompartilhamento = string.Format(linkAceitarConviteCompartilhamento, convite.TokenConvite);
            mensagemEmail = mensagemEmail.Replace("#LINK_ACEITAR_CONVITE#", linkAceitarConviteCompartilhamento);
            //
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

            EmailNegocio.Instance(_configuration).EnviarEmail(convite.EmailConvidado, convite.NomeConvidado, "Gerenciador Financeiro - Convite de Compartilhamento", mensagemEmail, anexos);

        }

        /// <summary>
        /// Registrar convite de compartilhamento, gerando o token de conferência para aceite do convite.
        /// </summary>
        /// <param name="uofw">Instância do banco aberta para transação, tudo ou nada.</param>
        /// <param name="convite">Convite de compartilhamento para registro.</param>
        /// <returns></returns>
        private ConviteCompartilhamento RegistrarConvites(IUnitOfWork uofw, ConviteCompartilhamento convite)
        {
            convite.DataHoraRegistroConvite = DateTime.Now;
            convite.DataHoraAceiteConvite = null;
            convite.TokenConvite = UtilNegocio.GerarSaltPassword();
            uofw.ConviteCompartilhamento.Incluir(convite);
            return convite;
        }

        /// <summary>
        /// Realiza a validação das informações do convite de compartilhamento antes de seu registro.
        /// </summary>
        /// <param name="convite">Informações do convite para validação.</param>
        /// <returns></returns>
        private void Validar(ConviteCompartilhamento convite)
        {
            if (convite == null)
            {
                throw new NegocioException("Convite de compartilhamento informado nulo.");
            }
            if (convite.IdEntidade == 0)
            {
                convite.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(convite.NomeConvidado))
            {
                convite.NomeConvidado = "Amigo(a)";
            }

            if (String.IsNullOrEmpty(convite.EmailConvidado))
            {
                throw new NegocioException("E-mail do convidado não informado.");
            }

            if (!UtilNegocio.ValidarEmail(convite.EmailConvidado))
            {
                throw new NegocioException("E-mail do convidade [{0}] é invalido.");
            }

            if (convite.CodigoPermissaoCompartilhamento == 0)
            {
                throw new NegocioException("Permissão de compartilhamento não informada (Editar ou Visualizar).");
            }

        }

        /// <summary>
        /// Recupera um convite de compartilhamento pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do convite de compartilhamento para recuperação.</param>
        /// <returns></returns>
        public ConviteCompartilhamento ObterConvite(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.ConviteCompartilhamento.ObterPorId(id);
            }
        }

    }
}
