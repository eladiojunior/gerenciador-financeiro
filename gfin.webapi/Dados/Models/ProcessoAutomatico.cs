namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_PROCESSO_AUTOMATICO")]
    public partial class ProcessoAutomatico
    {
        
        [Key]
        [Column("ID_PROCESSO_AUTOMATICO")]
        public int Id { get; set; }

        [Required]
        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Required]
        [Column("CD_TIPO_PROCESSO_AUTOMATICO")]
        public short CodigoTipoProcessoAutomatico { get; set; }

        [Required]
        [StringLength(80)]
        [Column("NM_PROCESSO_AUTOMATICO")]
        public string NomeProcessoAutomatico { get; set; }

        [Column("CD_TIPO_SITUACAO_ATUAL_PROCESSO")]
        public short CodigoTipoSituacaoAtualProcesso { get; set; }

        [Column("DH_REGISTRO_PROCESSO_AUTOMATICO")]
        public DateTime DataHoraRegistroProcesso { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
