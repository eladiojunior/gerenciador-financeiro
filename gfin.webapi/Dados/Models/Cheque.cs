namespace gfin.webapi.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_CHEQUE_CONTA")]
    public partial class Cheque
    {
        public Cheque()
        {
            ListaChequeCancelado = new HashSet<ChequeCancelado>();
            ListaChequeCompensado = new HashSet<ChequeCompensado>();
            ListaChequeDevolvido = new HashSet<ChequeDevolvido>();
            ListaChequeEmitido = new HashSet<ChequeEmitido>();
            ListaChequeResgatado = new HashSet<ChequeResgatado>();
        }

        [Key]
        [Column("ID_CHEQUE_CONTA")]
        public int Id { get; set; }

        [Column("ID_CONTA_CORRENTE")]
        public int IdContaCorrente { get; set; }

        [Required]
        [Column("NR_CHEQUE_CONTA")]
        public int NumeroCheque { get; set; }

        [Required]
        [Column("CD_TIPO_SITUACAO_CHEQUE")]
        public short CodigoSituacaoCheque { get; set; }

        [Column("DH_REGISTRO_CHEQUE_CONTA")]
        public DateTime DataHoraRegistroCheque { get; set; }

        [ForeignKey("IdContaCorrente")]
        public virtual ContaCorrente ContaCorrente { get; set; }

        public virtual ICollection<ChequeCancelado> ListaChequeCancelado { get; set; }
        public virtual ICollection<ChequeCompensado> ListaChequeCompensado { get; set; }
        public virtual ICollection<ChequeDevolvido> ListaChequeDevolvido { get; set; }
        public virtual ICollection<ChequeEmitido> ListaChequeEmitido { get; set; }
        public virtual ICollection<ChequeResgatado> ListaChequeResgatado { get; set; }

    }
}
