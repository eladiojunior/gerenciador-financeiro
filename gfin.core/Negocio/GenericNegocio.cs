using GFin.Dados;
using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFin.Negocio
{
    /// <summary>
    /// Classe genérica para centralizar os métodos utilizados pelas classes de negócio.
    /// </summary>
    public class GenericNegocio
    {
        private IClienteListener _clientListener = null;

        public GenericNegocio(IClienteListener clientListener)
        {
            this._clientListener = clientListener;
        }

        /// <summary>
        /// Verifica se as informações do Cliente foram enviadas na classe.
        /// </summary>
        /// <returns></returns>
        internal bool IsClienteListener()
        {
            return (this._clientListener != null);
        }

        /// <summary>
        /// Propriedade para recuperação das informações da camada de interface.
        /// Esse padrão é conhecido como DI (Dependency Injection) é um padrão de 
        /// design de software que nos permite desenvolver códigos fracamente acoplados. 
        /// DI é uma ótima maneira de reduzir o acoplamento apertado entre componentes 
        /// de software. 
        /// </summary>
        internal IClienteListener ClienteListener { 
            get {
                return this._clientListener;
            } 
        }

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
                using (IUnitOfWork uofw = new UnitOfWork())
                {
                    uofw.HistoricoUsuario.Incluir(historicoUsuario);
                    uofw.SalvarAlteracoes();
                }
            }
            catch (Exception) {} //Histórico não registrado, por erro.
        }

    }
}
