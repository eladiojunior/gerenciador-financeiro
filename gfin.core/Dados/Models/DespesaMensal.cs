namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    
    [Table("TB_DESPESA_MENSAL")]
    public partial class DespesaMensal
    {
        
        [Key]
        [Column("ID_DESPESA_MENSAL")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Column("ID_DESPESA_FIXA")]
        public int? IdDespesaFixa { get; set; }

        [Required]
        [Column("ID_NATUREZA_DESPESA")]
        public int IdNaturezaContaDespesa { get; set; }

        [Column("IN_DESPESA_PARCELADA")]
        public bool IsDespesaParcelada { get; set; }

        /// <summary>
        /// Identificador, gerado pelo sistema, para controlar as despesas parceladas
        /// de forma a manter um vínculo entre elas, para futura atualização ou remoção.
        /// </summary>
        [Column("CD_VINCULO_DESPESA_PARCELADA")]
        public int? CodigoDespesaParcelada { get; set; }

        [Required]
        [StringLength(150)]
        [Column("TX_DESCRICAO_DESPESA")]
        public string DescricaoDespesa { get; set; }

        [Required]
        [Column("DT_VENCIMENTO_DESPESA", TypeName="datetime")]
        public DateTime DataVencimentoDespesa { get; set; }

        [Required]
        [Column("VL_DESPESA")]
        public decimal ValorDespesa { get; set; }

        [Required]
        [Column("CD_FORMA_LIQUIDACAO_DESPESA")]
        public short CodigoTipoFormaLiquidacao { get; set; }

        /// <summary>
        /// Identificador que relaciona a forma de liquisação com o registro de origem.
        /// Ex.: Para a forma de liquidação seja "Cheque" aqui terá o Id do Cheque utilizado,
        /// caso seja "Cartão de Crédito" aqui terá o Id do cartão utilizado.
        /// Esse atributo pode ser NULL, caso o usuário não queira vincular nada;
        /// </summary>
        [Column("CD_VINCULO_FORMA_LIQUIDACAO")]
        public int? CodigoVinculoFormaLiquidacao { get; set; }

        [Column("IN_DESPESA_LIQUIDADA")]
        public bool IsDespesaLiquidada { get; set; }
        
        [StringLength(500)]
        [Column("TX_OBSERVACAO_DESPESA")]
        public string TextoObservacaoDespesa { get; set; }

        [Column("DH_LIQUIDACAO_DESPESA")]
        public DateTime? DataHoraLiquidacaoDespesa { get; set; }
        
        [Column("VL_DESCONTO_LIQUIDACAO_DESPESA")]
        public decimal? ValorDescontoLiquidacaoDespesa { get; set; }
        
        [Column("VL_MULTA_JUROS_LIQUIDACAO_DESPESA")]
        public decimal? ValorMultaJurosLiquidacaoDespesa { get; set; }
        
        [Column("VL_TOTAL_LIQUIDACAO_DESPESA")]
        public decimal? ValorTotalLiquidacaoDespesa { get; set; }
        
        [Column("DH_REGISTRO_DESPESA")]
        public DateTime DataHoraRegistroDespesa { get; set; }

        [ForeignKey("IdDespesaFixa")]
        public virtual DespesaFixa DespesaFixa { get; set; }

        [ForeignKey("IdNaturezaContaDespesa")]
        public virtual NaturezaConta NaturezaContaDespesa { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
