using GFin.Negocio.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web;

namespace GFin.Web
{
    public class TimerVerificarContasFixas
    {
        private static TimerVerificarContasFixas instancia = null;
        private const int constTempoVerificacao = (60000*30); //30 minutos

        public DateTime? DataUltimaVerificacao { get; private set; }
        public bool IsVerificando { get; private set; } = false;

        private TimerVerificarContasFixas() {
            System.Timers.Timer timerVerificacao = new System.Timers.Timer();
            timerVerificacao.Elapsed += OnTimedEvent;
            timerVerificacao.Interval = constTempoVerificacao;
            timerVerificacao.Enabled = true;
        }

        public static TimerVerificarContasFixas Instancia()
        {
            if (instancia == null)
            {
                instancia = new TimerVerificarContasFixas();
            }
            return instancia;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            if (IsVerificando)
            {
                System.Diagnostics.Debug.WriteLine("Processo de verificação em execução...");
                return;
            }

            if (DataUltimaVerificacao.HasValue)
                System.Diagnostics.Debug.WriteLine(string.Format("{0} - Data/hora da última verificação das contas fixas.", DataUltimaVerificacao.Value.ToString("dd/MM/yyyy HH:mm:ss")));
            else
                System.Diagnostics.Debug.WriteLine("Nenhuma verificação realizada anteriormente.");

            try
            { 

                VerificarContasFixasDoMes();

            }
            catch (Exception erro)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Erro >> {0}", erro.Message));
                IsVerificando = false;
                return;
            }

            DataUltimaVerificacao = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(string.Format("{0} - Verificação das contas fixas finalizada.", DataUltimaVerificacao.Value.ToString("dd/MM/yyyy HH:mm:ss")));

        }

        private void VerificarContasFixasDoMes()
        {

            IsVerificando = true;

            //Recuperar lista de entidades registradas na aplicação, essa verificação deverá ser realizada para todas as entidades...
            GFin.Negocio.EntidadeControleNegocio entidadeNegocio = new GFin.Negocio.EntidadeControleNegocio(null);
            var listaEntidades = entidadeNegocio.ListarEntidadeControles();
            foreach (var item in listaEntidades)
            {
                VerificarContasFixasDoMesPorEntidade(item.Id);
            }

            IsVerificando = false; //Finalizar a verificação...

        }

        internal void VerificarContasFixasDoMesPorEntidade(int idEntidade)
        {
            GFin.Negocio.DespesaNegocio despesaNegocio = new GFin.Negocio.DespesaNegocio(null);
            GFin.Negocio.ReceitaNegocio receitaNegocio = new GFin.Negocio.ReceitaNegocio(null);

            //Verificar a existência de despesas e receitas fixas não registradas...
            if (!receitaNegocio.IsReceitasFixasParaRegistroNoMesCorrente(idEntidade) &&
                !despesaNegocio.IsDespesasFixasParaRegistroNoMesCorrente(idEntidade))
            {
                IsVerificando = false;
                return;
            }

            string mesCorrente = DateTime.Now.ToString("MM/yyyy");
            GFin.Dados.Models.ProcessoAutomatico processo = new Dados.Models.ProcessoAutomatico();
            processo.IdEntidade = idEntidade;
            processo.NomeProcessoAutomatico = string.Format("Verificar Despesas e Receitas Fixas do mês/ano [{0}]", mesCorrente);
            processo.CodigoTipoProcessoAutomatico = (short)GFin.Dados.Enums.TipoProcessoAutomaticoEnum.ContasFixasMensal;

            GFin.Negocio.ProcessoAutomaticoNegocio processoAutomaticoNegocio = new GFin.Negocio.ProcessoAutomaticoNegocio(null);
            processo = processoAutomaticoNegocio.RegistrarProcesso(processo);

            try
            {

                //Manter Histórico.
                processoAutomaticoNegocio.RegistrarHistoricoProcesso(processo.Id, Dados.Enums.TipoSituacaoProcessoEnum.EmProcessamento, string.Format("Processando as contas fixas do mês/ano [{0}].", mesCorrente));

                int qtdDespesas = despesaNegocio.VerificarDespesasFixasDoMes(idEntidade);
                int qtdReceitas = receitaNegocio.VerificarReceitasFixasDoMes(idEntidade);

                StringBuilder sbHistorico = new StringBuilder();
                sbHistorico.AppendLine(string.Format("No mês/ano [{0}] corrente, foram encontrados:", mesCorrente));
                sbHistorico.AppendLine(string.Format("[{0}] despesa(s) fixa(s) para registro no mês/ano corrente;", qtdDespesas));
                sbHistorico.AppendLine(string.Format("[{0}] receita(s) fixa(s) para registro no mês/ano corrente;", qtdReceitas));

                //Manter Histórico.
                processoAutomaticoNegocio.RegistrarHistoricoProcesso(processo.Id, Dados.Enums.TipoSituacaoProcessoEnum.Processado, sbHistorico.ToString());

            }
            catch (Exception erro)
            {
                processoAutomaticoNegocio.RegistrarHistoricoProcesso(processo.Id, Dados.Enums.TipoSituacaoProcessoEnum.Erro, string.Format("Erro no processamento: {0}", erro.Message));
            }
        }
    }
}