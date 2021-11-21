namespace gfin.webapi.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_DESPESA_FIXA")]
    public partial class DespesaFixa
    {
        public DespesaFixa()
        {
            ListaDespesaMensal = new HashSet<DespesaMensal>();
            ListaHistoricoDespesaFixa = new HashSet<HistoricoDespesaFixa>();
        }

        [Key]
        [Column("ID_DESPESA_FIXA")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Required]
        [Column("ID_NATUREZA_CONTA_DESPESA_FIXA")]
        public int IdNaturezaContaDespesaFixa { get; set; }

        [Required]
        [StringLength(150)]
        [Column("TX_DESCRICAO_DESPESA_FIXA")]
        public string DescricaoDespesaFixa { get; set; }

        [Required]
        [Column("DD_VENCIMENTO_DESPESA_FIXA")]
        public short DiaVencimentoDespesaFixa { get; set; }

        [Required]
        [Column("VL_DESPESA_FIXA")]
        public decimal ValorDespesaFixa { get; set; }

        [Required]
        [Column("CD_TIPO_SITUACAO_DESPESA_FIXA")]
        public short CodigoTipoSituacaoDespesaFixa { get; set; }

        [Required]
        [Column("CD_FORMA_LIQUIDACAO_DESPESA_FIXA")]
        public short CodigoTipoFormaLiquidacaoDespesaFixa { get; set; }

        [Required]
        [Column("DH_REGISTRO_DESPESA_FIXA")]
        public DateTime DataHoraRegistroDespesaFixa { get; set; }

        [ForeignKey("IdNaturezaContaDespesaFixa")]
        public virtual NaturezaConta NaturezaContaDespesaFixa { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

        public virtual ICollection<DespesaMensal> ListaDespesaMensal { get; set; }
        public virtual ICollection<HistoricoDespesaFixa> ListaHistoricoDespesaFixa { get; set; }

    }
}
