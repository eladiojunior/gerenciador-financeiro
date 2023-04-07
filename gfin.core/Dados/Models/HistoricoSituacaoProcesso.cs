namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TB_HISTORICO_SITUACAO_PROCESSO")]
    public partial class HistoricoSituacaoProcesso
    {

        [Key]
        [Column("ID_HISTORICO_SITUACAO_PROCESSO")]
        public int Id { get; set; }

        [Required]
        [Column("ID_PROCESSO_AUTOMATICO")]
        public int IdProcessoAutomatico { get; set; }

        [Required]
        [Column("CD_TIPO_SITUACAO_PROCESSO")]
        public short CodigoTipoSituacaoProcesso { get; set; }

        [Required]
        [Column("DH_SITUACAO_PROCESSO")]
        public DateTime DataHoraSituacaoProcesso { get; set; }

        [StringLength(500)]
        [Column("TX_HISTORICO_SITUACAO_PROCESSO")]
        public string TextoHistoricoSituacaoProcesso { get; set; }

        [ForeignKey("IdProcessoAutomatico")]
        public virtual ProcessoAutomatico ProcessoAutomatico { get; set; }

    }
}
