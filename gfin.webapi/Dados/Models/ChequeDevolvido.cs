namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_CHEQUE_CONTA_DEVOLVIDO")]
    public partial class ChequeDevolvido
    {
        [Key]
        [Column("ID_CHEQUE_DEVOLVIDO")]
        public int Id { get; set; }

        [Column("ID_CHEQUE_CONTA")]
        public int IdCheque { get; set; }

        [Column("DT_DEVOLUCAO_CHEQUE")]
        public DateTime DataDevolucaoCheque { get; set; }

        [Required]
        [StringLength(500)]
        [Column("TX_OBSERVACAO_DEVOLUCAO_CHEQUE")]
        public string ObservacaoDevolucaoCheque { get; set; }

        [ForeignKey("IdCheque")]
        public virtual Cheque Cheque { get; set; }
    }
}
