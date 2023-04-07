namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TB_CHEQUE_CONTA_RESGATADO")]
    public partial class ChequeResgatado
    {
        [Key]
        [Column("ID_CHEQUE_RESGATADO")]
        public int Id { get; set; }
        
        [Column("ID_CHEQUE_CONTA")]
        public int IdCheque { get; set; }
        
        [Column("DT_RESGATE_CHEQUE")]
        public DateTime DataResgateCheque { get; set; }
        
        [Column("IN_BAIXA_CHEQUE_CCF")]
        public bool IsBaixaChequeCCF { get; set; }
        
        [Column("DT_BAIXA_CHEQUE_CCF")]
        public DateTime? DataBaixaChequeCCF { get; set; }
        
        [Column("VL_BAIXA_CHEQUE_CCF")]
        public decimal ValorBaixaChequeCCF { get; set; }
        
        [Column("VL_RESGATE_CHEQUE")]
        public decimal ValorResgateCheque { get; set; }

        [StringLength(500)]
        [Column("TX_OBSERVACAO_RESGATE_CHEQUE")]
        public string ObservacaoResgateCheque { get; set; }

        [ForeignKey("IdCheque")]
        public virtual Cheque Cheque { get; set; }
    }
}
