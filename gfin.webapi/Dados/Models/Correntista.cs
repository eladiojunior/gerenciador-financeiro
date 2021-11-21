using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gfin.webapi.Dados.Models
{
    [Table("TB_AGENDA_CORRENTISTA")]
    public class Correntista
    {
        [Key]
        [Column("ID_AGENDA_CORRENTISTA")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }
        
        [Required]
        [StringLength(80)]
        [Column("NM_BANCO")]
        public string NomeBanco { get; set; }
        
        [Required]
        [StringLength(20)]
        [Column("NR_AGENCIA")]
        public string NumeroAgencia { get; set; }
        
        [Required]
        [StringLength(30)]
        [Column("NR_CONTA_CORRENTE")]
        public string NumeroContaCorrente { get; set; }
        
        [Required]
        [StringLength(80)]
        [Column("NM_CORRENTISTA")]
        public string NomeCorrentista { get; set; }

        [StringLength(250)]
        [Column("TX_OBSERVACAO_CORRENTISTA")]
        public string Observacao { get; set; }

        [Column("DH_REGISTRO_CORRENTISTA")]
        public DateTime DataHoraRegistro { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
