namespace GFin.Dados
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using GFin.Dados.Models;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class GFinContext : DbContext
    {
        public GFinContext()
            : base("name=GFinContext")
        {
#if DEBUG
            Database.Log = GravarLogEntity;
#endif
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        private static void GravarLogEntity(string log)
        {
            System.Diagnostics.Debug.WriteLine(log);
        }

        public virtual DbSet<DespesaFixa> DespesaFixa { get; set; }
        public virtual DbSet<HistoricoDespesaFixa> HistoricoDespesaFixa { get; set; }
        public virtual DbSet<DespesaMensal> DespesaMensal { get; set; }
        public virtual DbSet<NaturezaConta> NaturezaConta { get; set; }
        public virtual DbSet<ReceitaFixa> ReceitaFixa { get; set; }
        public virtual DbSet<ReceitaMensal> ReceitaMensal { get; set; }
        public virtual DbSet<UsuarioSistema> UsuarioSistema { get; set; }
        public virtual DbSet<ContaCorrente> ContaCorrente { get; set; }
        public virtual DbSet<Cheque> Cheque { get; set; }
        public virtual DbSet<ChequeCancelado> ChequeCancelado { get; set; }
        public virtual DbSet<ChequeCompensado> ChequeCompensado { get; set; }
        public virtual DbSet<ChequeDevolvido> ChequeDevolvido { get; set; }
        public virtual DbSet<ChequeEmitido> ChequeEmitido { get; set; }
        public virtual DbSet<ChequeResgatado> ChequeResgatado { get; set; }
        public virtual DbSet<Correntista> Correntista { get; set; }
        public virtual DbSet<CartaoCredito> CartaoCredito { get; set; }
        public virtual DbSet<EntidadeControle> EntidadeControle { get; set; }
        public virtual DbSet<UsuarioAcessoEntidadeControle> UsuarioAcessoEntidadeControle { get; set; }
        public virtual DbSet<HistoricoUsuario> HistoricoUsuario { get; set; }

        public virtual DbSet<ProcessoAutomatico> ProcessoAutomatico { get; set; }
        public virtual DbSet<HistoricoSituacaoProcesso> HistoricoSituacaoProcesso { get; set; }

        public virtual DbSet<ConviteCompartilhamento> ConviteCompartilhamento { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //Mapeamento da classe AgendaCorrentista
            modelBuilder.Entity<Correntista>()
                .Property(e => e.NomeBanco)
                .IsUnicode(false);
            modelBuilder.Entity<Correntista>()
                .Property(e => e.NumeroAgencia)
                .IsUnicode(false);
            modelBuilder.Entity<Correntista>()
                .Property(e => e.NumeroContaCorrente)
                .IsUnicode(false);
            modelBuilder.Entity<Correntista>()
                .Property(e => e.NomeCorrentista)
                .IsUnicode(false);
            modelBuilder.Entity<Correntista>()
                .Property(e => e.Observacao)
                .IsUnicode(false);

            //Mapeamento da classe CartaoCredito
            modelBuilder.Entity<CartaoCredito>()
                .Property(e => e.NumeroCartaoCredito)
                .IsUnicode(false);
            modelBuilder.Entity<CartaoCredito>()
                .Property(e => e.NomeCartaoCredito)
                .IsUnicode(false);
            modelBuilder.Entity<CartaoCredito>()
                .Property(e => e.ValorLimiteCartaoCredito)
                .HasPrecision(15, 2);
            modelBuilder.Entity<CartaoCredito>()
                .Property(e => e.NomeProprietarioCartaoCredito)
                .IsUnicode(false);

            //Mapeamento da classe Cheque... 
            modelBuilder.Entity<Cheque>()
                .HasMany(e => e.ListaChequeCancelado)
                .WithRequired(e => e.Cheque)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Cheque>()
                .HasMany(e => e.ListaChequeCompensado)
                .WithRequired(e => e.Cheque)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Cheque>()
                .HasMany(e => e.ListaChequeDevolvido)
                .WithRequired(e => e.Cheque)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Cheque>()
                .HasMany(e => e.ListaChequeEmitido)
                .WithRequired(e => e.Cheque)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Cheque>()
                .HasMany(e => e.ListaChequeResgatado)
                .WithRequired(e => e.Cheque)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<ChequeCancelado>()
                .Property(e => e.ObservacaoCancelamentoCheque)
                .IsUnicode(false);
            modelBuilder.Entity<ChequeCompensado>()
                .Property(e => e.ObservacaoChequeCompensado)
                .IsUnicode(false);
            modelBuilder.Entity<ChequeDevolvido>()
                .Property(e => e.ObservacaoDevolucaoCheque)
                .IsUnicode(false);
            modelBuilder.Entity<ChequeEmitido>()
                .Property(e => e.HistoricoEmissaoCheque)
                .IsUnicode(false);
            modelBuilder.Entity<ChequeEmitido>()
                .Property(e => e.ValorChequeEmitido)
                .HasPrecision(15, 2);
            modelBuilder.Entity<ChequeResgatado>()
                .Property(e => e.ValorBaixaChequeCCF)
                .HasPrecision(15, 2);
            modelBuilder.Entity<ChequeResgatado>()
                .Property(e => e.ValorResgateCheque)
                .HasPrecision(15, 2);
            modelBuilder.Entity<ChequeResgatado>()
                .Property(e => e.ObservacaoResgateCheque)
                .IsUnicode(false);

            //Mapeamento da classe ContaCorrente
            modelBuilder.Entity<ContaCorrente>()
                .Property(e => e.NomeBanco)
                .IsUnicode(false);
            modelBuilder.Entity<ContaCorrente>()
                .Property(e => e.NumeroAgencia)
                .IsUnicode(false);
            modelBuilder.Entity<ContaCorrente>()
                .Property(e => e.NumeroContaCorrente)
                .IsUnicode(false);
            modelBuilder.Entity<ContaCorrente>()
                .Property(e => e.NomeTitularConta)
                .IsUnicode(false);
            modelBuilder.Entity<ContaCorrente>()
                .Property(e => e.ValorLimiteConta)
                .HasPrecision(15, 2);
            modelBuilder.Entity<ContaCorrente>()
                .HasMany(e => e.ListaCheque)
                .WithRequired(e => e.ContaCorrente)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<ContaCorrente>()
                .HasMany(e => e.ListaExtratoBancarioConta)
                .WithRequired(e => e.ContaCorrente)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<ContaCorrente>()
                .HasMany(e => e.ListaLancamentoExtratoBancario)
                .WithRequired(e => e.ContaCorrente)
                .WillCascadeOnDelete(false);


            //Mapeamento da classe Despesa Fixa e Mensal
            modelBuilder.Entity<DespesaFixa>()
                .Property(e => e.DescricaoDespesaFixa)
                .IsUnicode(false);
            modelBuilder.Entity<DespesaFixa>()
                .Property(e => e.ValorDespesaFixa)
                .HasPrecision(15, 2);
            modelBuilder.Entity<DespesaFixa>()
                .HasMany(e => e.ListaHistoricoDespesaFixa)
                .WithRequired(e => e.DespesaFixa)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<DespesaMensal>()
                .Property(e => e.DescricaoDespesa)
                .IsUnicode(false);
            modelBuilder.Entity<DespesaMensal>()
                .Property(e => e.ValorDespesa)
                .HasPrecision(15, 2);
            modelBuilder.Entity<DespesaMensal>()
                .Property(e => e.TextoObservacaoDespesa)
                .IsUnicode(false);
            modelBuilder.Entity<DespesaMensal>()
                .Property(e => e.ValorDescontoLiquidacaoDespesa)
                .HasPrecision(15, 2);
            modelBuilder.Entity<DespesaMensal>()
                .Property(e => e.ValorMultaJurosLiquidacaoDespesa)
                .HasPrecision(15, 2);
            modelBuilder.Entity<DespesaMensal>()
                .Property(e => e.ValorTotalLiquidacaoDespesa)
                .HasPrecision(15, 2);
            modelBuilder.Entity<HistoricoDespesaFixa>()
                .Property(e => e.ValorHistoricoDespesaFixa)
                .HasPrecision(15, 2);

            //Mapeamento da classe LancamentoExtratoBancario
            modelBuilder.Entity<LancamentoExtratoBancario>()
                .Property(e => e.HistoricoLancamentoExtrato)
                .IsUnicode(false);
            modelBuilder.Entity<LancamentoExtratoBancario>()
                .Property(e => e.NumeroLancamentoExtrato)
                .IsUnicode(false);
            modelBuilder.Entity<LancamentoExtratoBancario>()
                .Property(e => e.ValorLancamentoExtrato)
                .HasPrecision(15, 2);

            //Mapeamento da classe NaturezaConta
            modelBuilder.Entity<NaturezaConta>()
                .Property(e => e.DescricaoNaturezaConta)
                .IsUnicode(false);

            //Mapeamento da classe ReceitaFixa e Mensal
            modelBuilder.Entity<ReceitaFixa>()
                .Property(e => e.DescricaoReceitaFixa)
                .IsUnicode(false);
            modelBuilder.Entity<ReceitaFixa>()
                .Property(e => e.ValorReceitaFixa)
                .HasPrecision(15, 2);
            modelBuilder.Entity<ReceitaMensal>()
                .Property(e => e.TextoDescricaoReceita)
                .IsUnicode(false);
            modelBuilder.Entity<ReceitaMensal>()
                .Property(e => e.ValorReceita)
                .HasPrecision(15, 2);
            modelBuilder.Entity<ReceitaMensal>()
                .Property(e => e.ValorTotalLiquidacaoReceita)
                .HasPrecision(15, 2);
            
            //Mapeamento da classe UsuarioSistema
            modelBuilder.Entity<UsuarioSistema>()
                .Property(e => e.NomeUsuario)
                .IsUnicode(false);
            modelBuilder.Entity<UsuarioSistema>()
                .Property(e => e.EmailUsuario)
                .IsUnicode(false);
            modelBuilder.Entity<UsuarioSistema>()
                .Property(e => e.SenhaUsuario)
                .IsUnicode(false);
            modelBuilder.Entity<UsuarioSistema>()
                .Property(e => e.SaltSenhaUsuario)
                .IsUnicode(false);

            modelBuilder.Entity<EntidadeControle>()
                .Property(e => e.NomeEntidade)
                .IsUnicode(false);

            //Mapeamento da classe HistoricoUsuario
            modelBuilder.Entity<HistoricoUsuario>()
                .Property(e => e.IpMaquinaUsuario)
                .IsUnicode(false);
            modelBuilder.Entity<HistoricoUsuario>()
                .Property(e => e.DispositivoAcessoUsuario)
                .IsUnicode(false);
            modelBuilder.Entity<HistoricoUsuario>()
                .Property(e => e.TextoHistoricoUsuario)
                .IsUnicode(false);

            //Mapeamento da classe ProcessoAutomatico
            modelBuilder.Entity<ProcessoAutomatico>()
                .Property(e => e.NomeProcessoAutomatico)
                .IsUnicode(false);
            //Mapeamento da classe HistoricoSituacaoProcesso
            modelBuilder.Entity<HistoricoSituacaoProcesso>()
                .Property(e => e.TextoHistoricoSituacaoProcesso)
                .IsUnicode(false);

            //Mapeamento da classe ConviteCompartilhamento
            modelBuilder.Entity<ConviteCompartilhamento>()
                .Property(e => e.NomeConvidado)
                .IsUnicode(false);
            modelBuilder.Entity<ConviteCompartilhamento>()
                .Property(e => e.EmailConvidado)
                .IsUnicode(false);
            modelBuilder.Entity<ConviteCompartilhamento>()
                .Property(e => e.TokenConvite)
                .IsUnicode(false);
        }
    }
}
