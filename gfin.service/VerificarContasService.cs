using GFin.Negocio.Listeners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Service
{
    public partial class VerificarContasService : ServiceBase, IClienteListener
    {
        public VerificarContasService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("GFin.Service"))
            {
                System.Diagnostics.EventLog.CreateEventSource("GFin.Service", "GFin.VerificadorContasFixas");
            }
            eventLogGFin.Source = "GFin.Service";
            eventLogGFin.Log = "GFin.VerificadorContasFixas";
        }

        protected override void OnStart(string[] args)
        {
            const int temporizadorPadrao = 3600000*2; //Padrão: 2 horas.
            int temporizador = 0;
            if (args.Length == 0)
            {
                eventLogGFin.WriteEntry("Não foi definido o temporizador do processo: VerificadorContasFixas, será utilizado o valor padrão de 2 horas.", EventLogEntryType.Warning);
                temporizador = temporizadorPadrao;
            }
            else
            {
                var strTemporizador = (string)args[0];
                if (!Int32.TryParse(strTemporizador, out temporizador))
                {//Não foi possível converter o temporizador.
                    eventLogGFin.WriteEntry(string.Format("Valor do temporizador ({0}) informado é invalido! Será utilizado o valor padrão de 2 horas.", strTemporizador), EventLogEntryType.Error);
                    temporizador = temporizadorPadrao;
                }else if (temporizador < 600000)
                {//Identificado que o temporizador informado é menor que 10 min.
                    eventLogGFin.WriteEntry(string.Format("Valor do temporizador ({0}) menor que 10 minutos, será utilizado o valor padrão de 2 horas.", temporizador), EventLogEntryType.Error);
                    temporizador = temporizadorPadrao;
                }
            }

            eventLogGFin.WriteEntry("Processo: VerificadorContasFixas, inicializado.");
            
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = temporizador;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

        }

        protected override void OnStop()
        {
            eventLogGFin.WriteEntry("Processo: VerificadorContasFixas, finalizado.");
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            
            eventLogGFin.WriteEntry(string.Format("INICIO >> Verificar Contas Fixas: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), EventLogEntryType.Information));

            try 
	        {	        
                GFin.Negocio.DespesaNegocio despesaNegocio = new GFin.Negocio.DespesaNegocio(this);
                eventLogGFin.WriteEntry("Verificar as despesas fixas.");

                GFin.Negocio.EntidadeControleNegocio entidadeNegocio = new GFin.Negocio.EntidadeControleNegocio(this);
                var listaEntidade = entidadeNegocio.ListarEntidadeControles();
                
                int qtdTotalDespesasRegistradas = 0;
                foreach (var entidade in listaEntidade)
                {
                    int qtdDespesasRegistradas = despesaNegocio.VerificarDespesasFixasDoMes(entidade.Id);
                    eventLogGFin.WriteEntry(string.Format("Registradas {0} despesas fixas na entidade [{1}] CPF/CNPJ [{2}].", qtdDespesasRegistradas, entidade.CodigoTipoEntidade, entidade.CpfCnpjEntidade), EventLogEntryType.Information);
                    qtdTotalDespesasRegistradas += qtdDespesasRegistradas;
                }

                eventLogGFin.WriteEntry(string.Format("Foram registradas no total {0} despesas fixas.", qtdTotalDespesasRegistradas), EventLogEntryType.Information);

            }
	        catch (Exception erro)
	        {
                eventLogGFin.WriteEntry(string.Format("ERRO >> Registro despesas fixas, {0}", erro.Message), EventLogEntryType.Error);
	        }
            

            try 
            {	        
                GFin.Negocio.ReceitaNegocio receitaNegocio = new GFin.Negocio.ReceitaNegocio(this);
                eventLogGFin.WriteEntry("Verificar as receitas fixas.");

                GFin.Negocio.EntidadeControleNegocio entidadeNegocio = new GFin.Negocio.EntidadeControleNegocio(this);
                var listaEntidade = entidadeNegocio.ListarEntidadeControles();

                int qtdTotalReceitasRegistradas = 0;
                foreach (var entidade in listaEntidade)
                {
                    int qtdReceitasRegistradas = receitaNegocio.VerificarReceitasFixasDoMes(entidade.Id);
                    eventLogGFin.WriteEntry(string.Format("Registradas {0} receitas fixas na entidade [{1}] CPF/CNPJ [{2}].", qtdReceitasRegistradas, entidade.CodigoTipoEntidade, entidade.CpfCnpjEntidade), EventLogEntryType.Information);
                    qtdTotalReceitasRegistradas += qtdReceitasRegistradas;
                }

                eventLogGFin.WriteEntry(string.Format("Foram registradas no total {0} receitas fixas.", qtdTotalReceitasRegistradas), EventLogEntryType.Information);

            }
            catch (Exception erro)
            {
                eventLogGFin.WriteEntry(string.Format("ERRO >> Registro receitas fixas, {0}", erro.Message), EventLogEntryType.Error);
            }

            eventLogGFin.WriteEntry(string.Format("FINAL >> Verificar Contas Fixas: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), EventLogEntryType.Information));

        }

        public string IpMaquinaUsuario
        {
            get { 
                throw new NotImplementedException(); 
            }
        }

        public string InfoDispositivoUsuario
        {
            get { throw new NotImplementedException(); }
        }

        public IUsuarioLogado UsuarioLogado
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDispositivoMobileUsuario
        {
            get { return false; }
        }


        public string MapPathAppServidor
        {
            get { return "C:\\TEMP\\"; }
        }
    }
}
