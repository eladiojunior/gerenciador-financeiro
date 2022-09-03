using GFin.Dados;
using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GFin.Negocio
{
    public class ProcessoAutomaticoNegocio : GenericNegocio
    {
        public ProcessoAutomaticoNegocio(GFin.Negocio.Listeners.IClienteListener clienteListener) : base(clienteListener) { }

        /// <summary>
        /// Registra um processo automático valida.
        /// </summary>
        /// <param name="processo">Informações do processo automático a ser registrada.</param>
        /// <returns></returns>
        public ProcessoAutomatico RegistrarProcesso(ProcessoAutomatico processo)
        {
            ValidarProcesso(processo);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                
                //Criar processo.
                processo.DataHoraRegistroProcesso = DateTime.Now;
                processo.CodigoTipoSituacaoAtualProcesso = (short)TipoSituacaoProcessoEnum.Criado;
                processo = uofw.ProcessoAutomatico.Incluir(processo);
                
                //Criar histórico do processo...
                HistoricoSituacaoProcesso historico = new HistoricoSituacaoProcesso();
                historico.IdProcessoAutomatico = processo.Id;
                historico.CodigoTipoSituacaoProcesso = (short)TipoSituacaoProcessoEnum.Criado;
                historico.DataHoraSituacaoProcesso = DateTime.Now;
                historico.TextoHistoricoSituacaoProcesso = "Processo de verificação criado automaticamente.";
                historico = uofw.HistoricoSituacaoProcesso.Incluir(historico);

                uofw.SalvarAlteracoes();
                return processo;
            }
        }

        /// <summary>
        /// Valida as informações do processo automático.
        /// </summary>
        /// <param name="processo">Informações da natureza a ser validada.</param>
        private void ValidarProcesso(ProcessoAutomatico processo)
        {
            if (processo == null)
            {
                throw new NegocioException("Nenhuma informação do processo automático preenchida.");
            }
            if (processo.IdEntidade == 0)
            {
                processo.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(processo.NomeProcessoAutomatico))
            {
                throw new NegocioException("Nome do processo automático não informado.");
            }
            processo.NomeProcessoAutomatico = processo.NomeProcessoAutomatico.Trim();

            if (processo.CodigoTipoProcessoAutomatico == (int)TipoProcessoAutomaticoEnum.NaoInformado)
            {
                throw new NegocioException("Tipo do processo automático não informado.");
            }
            if (!UtilEnum.IsEnumValido(typeof(TipoProcessoAutomaticoEnum), processo.CodigoTipoProcessoAutomatico))
            {
                throw new NegocioException(string.Format("Tipo do processo automático [{0}] inválido.", processo.CodigoTipoProcessoAutomatico));
            }
            //Verificar a existê de uma Processo automático "Em Processamento" para essa mesma entidade.
            if (processo.Id == 0)
            {//Nova

                using (IUnitOfWork uofw = new UnitOfWork())
                {
                    var qtdRegs = uofw.ProcessoAutomatico.QuantRegistros(n => n.IdEntidade == processo.IdEntidade && 
                        n.CodigoTipoProcessoAutomatico == processo.CodigoTipoProcessoAutomatico &&
                        n.CodigoTipoSituacaoAtualProcesso == (short)TipoSituacaoProcessoEnum.EmProcessamento);
                    if (qtdRegs > 0)
                    {//Encontrado um processo automático "Em Processamento".
                        throw new NegocioException("Já existe um processo automático em processamento.");
                    }
                } 
            }
        }

        /// <summary>
        /// Recupera a lista de todas os processos automáticos da entidade;
        /// </summary>
        /// <returns></returns>
        public List<ProcessoAutomatico> ListarProcessos()
        {
            return ListarProcessos(TipoProcessoAutomaticoEnum.NaoInformado);
        }

        /// <summary>
        /// Recupera a lista de processos automáticos pelo tipo informado;
        /// </summary>
        /// <param name="tipoProcesso">Tipo do processo automático.</param>
        /// <returns></returns>
        public List<ProcessoAutomatico> ListarProcessos(TipoProcessoAutomaticoEnum tipoProcesso)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                int _idEntidade = ClienteListener.UsuarioLogado.IdEntidade;
                if (tipoProcesso == TipoProcessoAutomaticoEnum.NaoInformado)
                {//Recupera todos os tipos...
                    return uofw.ProcessoAutomatico.Listar(n => n.IdEntidade == _idEntidade, 
                        n => n.DataHoraRegistroProcesso, false);
                }

                //Recuperar informação específica... por tipo de processo;
                return uofw.ProcessoAutomatico.Listar(n => n.IdEntidade == _idEntidade && 
                    n.CodigoTipoProcessoAutomatico == (short)tipoProcesso, 
                    n => n.DataHoraRegistroProcesso, false);
            }

        }

        /// <summary>
        /// Recupera a lista de processos automáticos pelo tipo informado, informando a quantidade de registro que seram retornados;
        /// </summary>
        /// <param name="tipoProcesso">Tipo do processo automático.</param>
        /// <param name="qtdMaximaRegistros">Indica a quantidade de registro que será retornado.</param>
        /// <returns></returns>
        public List<ProcessoAutomatico> ListarProcessos(TipoProcessoAutomaticoEnum tipoProcesso, int qtdMaximaRegistros)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                ListaPaginada<ProcessoAutomatico> paginacao = null;
                int _idEntidade = ClienteListener.UsuarioLogado.IdEntidade;
                if (tipoProcesso == TipoProcessoAutomaticoEnum.NaoInformado)
                {//Recupera todos os tipos...
                    paginacao = uofw.ProcessoAutomatico.Listar(n => n.IdEntidade == _idEntidade, 1, qtdMaximaRegistros, n => n.DataHoraRegistroProcesso, false);
                    return paginacao.Resultado;
                }

                //Recuperar informação específica... por tipo de processo;
                paginacao = uofw.ProcessoAutomatico.Listar(n => n.IdEntidade == _idEntidade && n.CodigoTipoProcessoAutomatico == (short)tipoProcesso, 1, qtdMaximaRegistros, n => n.DataHoraRegistroProcesso, false);
                return paginacao.Resultado;
            }
        }

        /// <summary>
        /// Grava as alterações realizadas no processo automático.
        /// </summary>
        /// <param name="processo">Objeto com as informações do processo.</param>
        public void GravarProcesso(ProcessoAutomatico processo)
        {
            ValidarProcesso(processo);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                uofw.ProcessoAutomatico.Alterar(processo);
                uofw.SalvarAlteracoes();
            } 
        }

        /// <summary>
        /// Recupera um processo automático pelo seu Identificador.
        /// </summary>
        /// <param name="idProcesso">Identificador do processo automático.</param>
        /// <returns></returns>
        public ProcessoAutomatico ObterProcesso(int idProcesso)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.ProcessoAutomatico.ObterPorId(idProcesso);
            } 
        }

        /// <summary>
        /// Recupera a lista de histórico da situação do processo automático pelo seu identificador.
        /// </summary>
        /// <param name="idProcesso">Identificador do processo automático.</param>
        /// <returns></returns>
        public List<HistoricoSituacaoProcesso> ListarHistoricoSituacaoProcesso(int idProcesso)
        {
            List<HistoricoSituacaoProcesso> historico = null;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                historico = uofw.HistoricoSituacaoProcesso.Listar(p => p.IdProcessoAutomatico == idProcesso, p => p.DataHoraSituacaoProcesso, false);
            }
            return historico;
        }

        /// <summary>
        /// Registra histórico de situação do processo, fluxo de vida do processo.
        /// </summary>
        /// <param name="idProcesso">Identificador do processo automático.</param>
        /// <param name="tipoSituacao">Código do tipo da situação do processo, Enum.</param>
        /// <param name="historicoSituacao">Texto [de 500 caracteres] contendo informação da situação que será registrada.</param>
        /// <returns></returns>
        public HistoricoSituacaoProcesso RegistrarHistoricoProcesso(int idProcesso, TipoSituacaoProcessoEnum tipoSituacao, string historicoSituacao)
        {
            if (idProcesso == 0)
            {
                throw new NegocioException("Identificador do processo não preenchido.");
            }
            ProcessoAutomatico _processo = ObterProcesso(idProcesso);
            if (_processo == null || _processo.Id == 0)
            {
                throw new NegocioException("Processo automático com Id [{0}] não encontrado.");
            }

            //Criar histórico do processo...
            using (IUnitOfWork uofw = new UnitOfWork())
            {

                //Incluir novo histórico.
                HistoricoSituacaoProcesso historico = new HistoricoSituacaoProcesso();
                historico.IdProcessoAutomatico = _processo.Id;
                historico.CodigoTipoSituacaoProcesso = (short)tipoSituacao;
                historico.DataHoraSituacaoProcesso = DateTime.Now;
                historico.TextoHistoricoSituacaoProcesso = historicoSituacao;
                historico = uofw.HistoricoSituacaoProcesso.Incluir(historico);
                
                //Atualizar situação atual do processo...
                _processo.CodigoTipoSituacaoAtualProcesso = (short)tipoSituacao;
                uofw.ProcessoAutomatico.Alterar(_processo);

                uofw.SalvarAlteracoes();
                return historico;
            }

        }
    }
}