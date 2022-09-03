namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TB_CHEQUE_CONTA_COMPENSADO")]
    public partial class ChequeCompensado
    {
        [Key]
        [Column("ID_CHEQUE_CONTA_COMPENSADO")]
        public int Id { get; set; }

        [Column("ID_CHEQUE_CONTA")]
        public int IdCheque { get; set; }

        [Column("DT_COMPENSACAO_CHEQUE_CONTA")]
        public DateTime DataCompensacaoCheque { get; set; }

        [StringLength(500)]
        [Column("TX_OBSERVACAO_CHEQUE_COMPENSADO")]
        public string ObservacaoChequeCompensado { get; set; }

        [ForeignKey("IdCheque")]
        public virtual Cheque Cheque { get; set; }
    }
}
