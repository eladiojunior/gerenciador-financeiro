using System;

namespace gfin.webapi.Dados
{
    /// <summary>
    /// Implementação do Unit of Work para encapsular os repositórios e ele
    /// irá montar uma estrutura de acesso aos repositórios e salvamento das 
    /// alterações realizadas neste repositórios.
    /// </summary>
    internal class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;

        private readonly GFinContext _context;

        private NaturezaContaDAO _naturezaConta;

        private DespesaMensalDAO _despesaMensal;
        private DespesaFixaDAO _despesaFixa;
        private HistoricoDespesaFixaDAO _historicoDespesaFixa;

        private ReceitaMensalDAO _receitaMensal;
        private ReceitaFixaDAO _receitaFixa;

        private ProcessoAutomaticoDAO _processoAutomatico;
        private HistoricoSituacaoProcessoDAO _historicoSituacaoProcesso;

        private CorrentistaDAO _correntista;
        private ContaCorrenteDAO _contaCorrente;
        private CartaoCreditoDAO _cartaoCredito;
        private ExtratoBancarioDAO _extratoBancario;
        private LancamentoExtratoBancarioDAO _lancamentoExtratoBancario;

        private ChequeDAO _cheque;
        private ChequeCanceladoDAO _chequeCancelado;
        private ChequeCompensadoDAO _chequeCompensado;
        private ChequeDevolvidoDAO _chequeDevolvido;
        private ChequeEmitidoDAO _chequeEmitido;
        private ChequeResgatadoDAO _chequeResgatado;

        private EntidadeControleDAO _entidadeControle;
        private UsuarioSistemaDAO _usuarioSistema;
        private UsuarioAcessoEntidadeControleDAO _usuarioAcessoEntidade;

        private HistoricoUsuarioDAO _historicoUsuario;

        private ConviteCompartilhamentoDAO _conviteCompartilhamento;

        public UnitOfWork(GFinContext context) {
            _context = context;
        }

        public void SalvarAlteracoes()
        {
            try
            {
                _context.SaveChangesAsync();
            }
            catch (Exception erro)
            {
                throw erro;
            }
        }

        public DespesaMensalDAO DespesaMensal => _despesaMensal ??= new DespesaMensalDAO(_context);

        public DespesaFixaDAO DespesaFixa => _despesaFixa ??= new DespesaFixaDAO(_context);

        public HistoricoDespesaFixaDAO HistoricoDespesaFixa
        {
            get
            {
                if (_historicoDespesaFixa == null)
                    _historicoDespesaFixa = new HistoricoDespesaFixaDAO(_context);
                return _historicoDespesaFixa;
            }
        }

        public ReceitaFixaDAO ReceitaFixa
        {
            get
            {
                if (_receitaFixa == null)
                    _receitaFixa = new ReceitaFixaDAO(_context);
                return _receitaFixa;
            }
        }

        public ReceitaMensalDAO ReceitaMensal
        {
            get
            {
                if (_receitaMensal == null)
                    _receitaMensal = new ReceitaMensalDAO(_context);
                return _receitaMensal;
            }
        }

        public ProcessoAutomaticoDAO ProcessoAutomatico
        {
            get
            {
                if (_processoAutomatico == null)
                    _processoAutomatico = new ProcessoAutomaticoDAO(_context);
                return _processoAutomatico;
            }
        }

        public HistoricoSituacaoProcessoDAO HistoricoSituacaoProcesso
        {
            get
            {
                if (_historicoSituacaoProcesso == null)
                    _historicoSituacaoProcesso = new HistoricoSituacaoProcessoDAO(_context);
                return _historicoSituacaoProcesso;
            }
        }

        public ChequeDAO Cheque
        {
            get
            {
                if (_cheque == null)
                    _cheque = new ChequeDAO(_context);
                return _cheque;
            }
        }

        public CorrentistaDAO Correntista
        {
            get
            {
                if (_correntista == null)
                    _correntista = new CorrentistaDAO(_context);
                return _correntista;
            }
        }

        public NaturezaContaDAO NaturezaConta
        {
            get
            {
                if (_naturezaConta == null)
                    _naturezaConta = new NaturezaContaDAO(_context);
                return _naturezaConta;
            }
        }

        public ContaCorrenteDAO ContaCorrente
        {
            get
            {
                if (_contaCorrente == null)
                    _contaCorrente = new ContaCorrenteDAO(_context);
                return _contaCorrente;
            }
        }

        public CartaoCreditoDAO CartaoCredito
        {
            get
            {
                if (_cartaoCredito == null)
                    _cartaoCredito = new CartaoCreditoDAO(_context);
                return _cartaoCredito;
            }
        }

        public ExtratoBancarioDAO ExtratoBancario
        {
            get
            {
                if (_extratoBancario == null)
                    _extratoBancario = new ExtratoBancarioDAO(_context);
                return _extratoBancario;
            }
        }

        public LancamentoExtratoBancarioDAO LancamentoExtratoBancario
        {
            get
            {
                if (_lancamentoExtratoBancario == null)
                    _lancamentoExtratoBancario = new LancamentoExtratoBancarioDAO(_context);
                return _lancamentoExtratoBancario;
            }
        }

        public ChequeCanceladoDAO ChequeCancelado
        {
            get
            {
                if (_chequeCancelado == null)
                    _chequeCancelado = new ChequeCanceladoDAO(_context);
                return _chequeCancelado;
            }
        }

        public ChequeCompensadoDAO ChequeCompensado
        {
            get
            {
                if (_chequeCompensado == null)
                    _chequeCompensado = new ChequeCompensadoDAO(_context);
                return _chequeCompensado;
            }
        }

        public ChequeDevolvidoDAO ChequeDevolvido
        {
            get
            {
                if (_chequeDevolvido == null)
                    _chequeDevolvido = new ChequeDevolvidoDAO(_context);
                return _chequeDevolvido;
            }
        }

        public ChequeEmitidoDAO ChequeEmitido
        {
            get
            {
                if (_chequeEmitido == null)
                    _chequeEmitido = new ChequeEmitidoDAO(_context);
                return _chequeEmitido;
            }
        }

        public ChequeResgatadoDAO ChequeResgatado
        {
            get
            {
                if (_chequeResgatado == null)
                    _chequeResgatado = new ChequeResgatadoDAO(_context);
                return _chequeResgatado;
            }
        }

        public UsuarioSistemaDAO UsuarioSistema
        {
            get
            {
                if (_usuarioSistema == null)
                    _usuarioSistema = new UsuarioSistemaDAO(_context);
                return _usuarioSistema;
            }
        }

        public UsuarioAcessoEntidadeControleDAO UsuarioAcessoEntidadeControle
        {
            get
            {
                if (_usuarioAcessoEntidade == null)
                    _usuarioAcessoEntidade = new UsuarioAcessoEntidadeControleDAO(_context);
                return _usuarioAcessoEntidade;
            }
        }

        public EntidadeControleDAO EntidadeControle
        {
            get
            {
                if (_entidadeControle == null)
                    _entidadeControle = new EntidadeControleDAO(_context);
                return _entidadeControle;
            }
        }

        public HistoricoUsuarioDAO HistoricoUsuario
        {
            get
            {
                if (_historicoUsuario == null)
                    _historicoUsuario = new HistoricoUsuarioDAO(_context);
                return _historicoUsuario;
            }
        }

        public ConviteCompartilhamentoDAO ConviteCompartilhamento
        {
            get
            {
                if (_conviteCompartilhamento == null)
                    _conviteCompartilhamento = new ConviteCompartilhamentoDAO(_context);
                return _conviteCompartilhamento;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
