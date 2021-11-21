namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_LANCAMENTO_EXTRATO_BANCARIO")]
    public partial class LancamentoExtratoBancario
    {
        [Key]
        [Column("ID_LANCAMENTO_EXTRATO")]
        public int Id { get; set; }

        [Column("ID_CONTA_CORRENTE")]
        public int IdContaCorrente { get; set; }

        [Column("CD_LANCAMENTO_CONTA")]
        public short CodigoTipoLancamentoExtrato { get; set; }

        [Column("DH_LANCAMENTO_EXTRATO")]
        public DateTime DataHoraLancamentoExtrato { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TX_HISTORICO_LANCAMENTO_EXTRATO")]
        public string HistoricoLancamentoExtrato { get; set; }

        [Required]
        [StringLength(30)]
        [Column("NR_LANCAMENTO_EXTRATO")]
        public string NumeroLancamentoExtrato { get; set; }

        [Column("VL_LANCAMENTO_EXTRATO")]
        public decimal ValorLancamentoExtrato { get; set; }

        [ForeignKey("IdContaCorrente")]
        public virtual ContaCorrente ContaCorrente { get; set; }
    }
}
