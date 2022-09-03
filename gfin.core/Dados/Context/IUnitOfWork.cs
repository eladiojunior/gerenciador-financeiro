using GFin.Dados;
using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
{
    /// <summary>
    /// Responsável pelo controle dos repositórios de dados, controlando as transações de banco de dados.
    /// </summary>
    internal interface IUnitOfWork : IDisposable
    {
        CorrentistaDAO Correntista { get; }

        NaturezaContaDAO NaturezaConta { get; }

        ContaCorrenteDAO ContaCorrente { get; }

        CartaoCreditoDAO CartaoCredito { get; }

        ExtratoBancarioDAO ExtratoBancario { get; }

        LancamentoExtratoBancarioDAO LancamentoExtratoBancario { get; }

        DespesaMensalDAO DespesaMensal { get; }
        DespesaFixaDAO DespesaFixa { get; }
        HistoricoDespesaFixaDAO HistoricoDespesaFixa { get; }

        ReceitaMensalDAO ReceitaMensal { get; }
        ReceitaFixaDAO ReceitaFixa { get; }
        
        ChequeDAO Cheque { get; }
        ChequeCanceladoDAO ChequeCancelado { get; }
        ChequeCompensadoDAO ChequeCompensado { get; }
        ChequeDevolvidoDAO ChequeDevolvido { get; }
        ChequeEmitidoDAO ChequeEmitido { get; }
        ChequeResgatadoDAO ChequeResgatado { get; }

        UsuarioSistemaDAO UsuarioSistema { get; }

        EntidadeControleDAO EntidadeControle { get; }

        HistoricoUsuarioDAO HistoricoUsuario { get; }
        
        ConviteCompartilhamentoDAO ConviteCompartilhamento { get; }

        UsuarioAcessoEntidadeControleDAO UsuarioAcessoEntidadeControle { get; }

        HistoricoSituacaoProcessoDAO HistoricoSituacaoProcesso { get; }

        ProcessoAutomaticoDAO ProcessoAutomatico { get; }

        void SalvarAlteracoes();

    }
}
