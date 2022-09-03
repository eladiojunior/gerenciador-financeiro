namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TB_EXTRATO_BANCARIO_CONTA")]
    public partial class ExtratoBancarioConta
    {
        [Key]
        [Column("ID_EXTRATO_BANCARIO")]
        public int Id { get; set; }

        [Column("ID_CONTA_CORRENTE")]
        public int IdContaCorrente { get; set; }

        [Column("DH_IMPORTACAO_EXTRATO")]
        public DateTime DataHoraImportacaoExtrato { get; set; }

        [Required]
        [StringLength(150)]
        [Column("TX_LOCAL_ARQUIVO_IMPORTACAO_EXTRATO")]
        public string LocalArquivoImportacaoExtrato { get; set; }

        [Column("IN_BAIXA_AUTOMATICA_DESPESA")]
        public bool IsBaixaAutomaticaDespesa { get; set; }

        [Column("IN_BAIXA_AUTOMATICA_RECEITA")]
        public bool IsBaixaAutomaticaReceita { get; set; }

        [Column("IN_BAIXA_AUTOMATICA_CHEQUE")]
        public bool IsBaixaAutomaticaCheque { get; set; }

        [StringLength(500)]
        [Column("TX_LOG_IMPORTACAO_EXTRATO")]
        public string LogImportacaoExtrato { get; set; }

        [ForeignKey("IdContaCorrente")]
        public virtual ContaCorrente ContaCorrente { get; set; }

    }
}
