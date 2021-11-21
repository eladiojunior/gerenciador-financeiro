namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_CHEQUE_CONTA_CANCELADO")]
    public partial class ChequeCancelado
    {
        [Key]
        [Column("ID_CHEQUE_CANCELADO")]
        public int Id { get; set; }

        [Column("ID_CHEQUE_CONTA")]
        public int IdCheque { get; set; }

        [Column("DT_CANCELAMENTO_CHEQUE")]
        public DateTime DataCancelamentoCheque { get; set; }

        [Column("IN_CANCELAMENTO_BANCO")]
        public bool IsCancelamentoBanco { get; set; }

        [StringLength(500)]
        [Column("TX_OBSERVACAO_CANCELAMENTO_CHEQUE")]
        public string ObservacaoCancelamentoCheque { get; set; }

        [ForeignKey("IdCheque")]
        public virtual Cheque Cheque { get; set; }
    }
}
