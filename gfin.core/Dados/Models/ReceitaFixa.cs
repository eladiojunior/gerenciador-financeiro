namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TB_RECEITA_FIXA")]
    public partial class ReceitaFixa
    {

        [Key]
        [Column("ID_RECEITA_FIXA")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Column("ID_NATUREZA_RECEITA_FIXA")]
        public int IdNaturezaContaReceitaFixa { get; set; }

        [Required]
        [StringLength(150)]
        [Column("TX_DESCRICAO_RECEITA_FIXA")]
        public string DescricaoReceitaFixa { get; set; }

        [Column("DD_RECEBIMENTO_RECEITA_FIXA")]
        public short DiaRecebimentoReceitaFixa { get; set; }

        [Column("VL_RECEITA_FIXA")]
        public decimal ValorReceitaFixa { get; set; }

        [Column("CD_SITUACAO_RECEITA_FIXA")]
        public short CodigoTipoSituacaoReceitaFixa { get; set; }

        [Column("DH_REGISTRO_RECEITA_FIXA")]
        public DateTime DataHoraRegistroReceitaFixa { get; set; }

        [ForeignKey("IdNaturezaContaReceitaFixa")]
        public virtual NaturezaConta NaturezaContaReceitaFixa { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
