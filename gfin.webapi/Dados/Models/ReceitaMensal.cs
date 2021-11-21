namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_RECEITA_MENSAL")]
    public partial class ReceitaMensal
    {
        [Key]
        [Column("ID_RECEITA_MENSAL")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Column("ID_RECEITA_FIXA")]
        public int? IdReceitaFixa { get; set; }

        [Required]
        [Column("ID_NATUREZA_CONTA")]
        public int IdNaturezaContaReceita { get; set; }

        [Required]
        [Column("CD_FORMA_RECEBIMENTO_RECEITA")]
        public short CodigoTipoFormaRecebimento { get; set; }

        [Required]
        [StringLength(150)]
        [Column("TX_DESCRICAO_RECEITA")]
        public string TextoDescricaoReceita { get; set; }

        [Required]
        [Column("DT_RECEBIMENTO_RECEITA")]
        public DateTime DataRecebimentoReceita { get; set; }

        [Required]
        [Column("VL_RECEITA")]
        public decimal ValorReceita { get; set; }

        [Column("IN_RECEITA_LIQUIDACAO")]
        public bool IsReceitaLiquidada { get; set; }

        [Column("DH_LIQUIDACAO_RECEITA")]
        public DateTime? DataHoraLiquidacaoReceita { get; set; }

        [Column("VL_TOTAL_LIQUIDACAO_RECEITA")]
        public decimal? ValorTotalLiquidacaoReceita { get; set; }

        [Column("DH_REGISTRO_RECEITA")]
        public DateTime DataHoraRegistroReceita { get; set; }

        [ForeignKey("IdReceitaFixa")]
        public virtual ReceitaFixa ReceitaFixa { get; set; }

        [ForeignKey("IdNaturezaContaReceita")]
        public virtual NaturezaConta NaturezaContaReceita { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}