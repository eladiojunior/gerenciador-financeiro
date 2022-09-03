namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TB_NATUREZA_CONTA")]
    public partial class NaturezaConta
    {
        
        [Key]
        [Column("ID_NATUREZA_CONTA")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Column("CD_LANCAMENTO_CONTA")]
        public short CodigoTipoLancamentoConta { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TX_DESCRICAO_NATUREZA_CONTA")]
        public string DescricaoNaturezaConta { get; set; }

        [Column("CD_SITUACAO_NATUREZA_CONTA")]
        public short CodigoTipoSituacaoNaturezaConta { get; set; }

        [Column("DH_REGISTRO_NATUREZA_CONTA")]
        public DateTime DataHoraRegistroNaturezaConta { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
