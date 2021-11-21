namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_CHEQUE_CONTA_EMITIDO")]
    public partial class ChequeEmitido
    {
        [Key]
        [Column("ID_CHEQUE_EMITIDO")]
        public int Id { get; set; }

        [Column("ID_CHEQUE_CONTA")]
        public int IdCheque { get; set; }

        [Column("DT_EMISSAO_CHEQUE_CONTA")]
        public DateTime DataEmissaoCheque { get; set; }

        [Column("DT_VENCIMENTO_CHEQUE_CONTA")]
        public DateTime DataVencimentoCheque { get; set; }

        [Required]
        [StringLength(150)]
        [Column("TX_HISTORICO_EMISSAO_CHEQUE_CONTA")]
        public string HistoricoEmissaoCheque { get; set; }

        [Column("VL_CHEQUE_EMITIDO")]
        public decimal ValorChequeEmitido { get; set; }

        [ForeignKey("IdCheque")]
        public virtual Cheque Cheque { get; set; }
    }
}
