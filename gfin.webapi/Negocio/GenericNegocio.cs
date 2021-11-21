using gfin.webapi.Dados;
using gfin.webapi.Dados.Enums;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio.Listeners;
using System;

namespace gfin.webapi.Negocio
{
    /// <summary>
    /// Classe genérica para centralizar os métodos utilizados pelas classes de negócio.
    /// </summary>
    public class GenericNegocio
    {
        private IClienteListener _clientListener = null;
        private GFinContext _context = null;
        public GenericNegocio(IClienteListener clientListener, GFinContext context)
        {
            _clientListener = clientListener;
            _context = context;
        }

        /// <summary>
        /// Verifica se as informações do Cliente foram enviadas na classe.
        /// </summary>
        /// <returns></returns>
        internal bool IsClienteListener()
        {
            return (_clientListener != null);
        }

        /// <summary>
        /// Propriedade para recuperação das informações da camada de interface.
        /// Esse padrão é conhecido como DI (Dependency Injection) é um padrão de 
        /// design de software que nos permite desenvolver códigos fracamente acoplados. 
        /// DI é uma ótima maneira de reduzir o acoplamento apertado entre componentes 
        /// de software. 
        /// </summary>
        internal IClienteListener ClienteListener => _clientListener;

        /// <summary>
        /// Proproedade com o contexto de conexão com o banco de dados.
        /// Esse padrão é conhecido como DI (Dependency Injection) é um padrão de 
        /// design de software que nos permite desenvolver códigos fracamente acoplados. 
        /// DI é uma ótima maneira de reduzir o acoplamento apertado entre componentes 
        /// de software. 
        /// </summary>
        internal GFinContext GFinContext => _context;

        /// <summary>
        /// Realiza a gravação de um Histórico do Usuário para um usuário específico (via Id).
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário para histórico.</param>
        /// <param name="tipoOperacaoHistorico">Tipo de Operação do Historico de Usuário [TipoOperacaoHistoricoUsuarioEnum].</param>
        /// <param name="textoHistorico">Texto do histórico do usuário.</param>
        /// <returns></returns>
        public void GravarHistoricoUsuario(int idUsuario, TipoOperacaoHistoricoUsuarioEnum tipoOperacaoHistorico, string textoHistorico)
        {
            try
            {

                HistoricoUsuario historicoUsuario = new HistoricoUsuario();
                historicoUsuario.IdUsuario = idUsuario;
                historicoUsuario.CodigoTipoOperacao = (short)tipoOperacaoHistorico;
                if (IsClienteListener())
                {
                    historicoUsuario.IpMaquinaUsuario = ClienteListener.IpMaquinaUsuario;
                    historicoUsuario.IsDispositivoMobileUsuario = ClienteListener.IsDispositivoMobileUsuario;
                    historicoUsuario.DispositivoAcessoUsuario = ClienteListener.InfoDispositivoUsuario;
                }
                historicoUsuario.TextoHistoricoUsuario = textoHistorico;
                historicoUsuario.DataHoraRegistroHistoricoUsuario = DateTime.Now;
                using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
                {
                    uofw.HistoricoUsuario.Incluir(historicoUsuario);
                    uofw.SalvarAlteracoes();
                }
            }
            catch (Exception) {} //Histórico não registrado, por erro.
        }

    }
}
