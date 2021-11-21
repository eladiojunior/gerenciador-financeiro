namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_HISTORICO_DESPESA_FIXA")]
    public partial class HistoricoDespesaFixa
    {

        [Key]
        [Column("ID_HISTORICO_DESPESA_FIXA")]
        public int Id { get; set; }

        [Column("ID_DESPESA_FIXA")]
        public int IdDespesaFixa { get; set; }

        [Column("VL_HISTORICO_DESPESA_FIXA")]
        public decimal ValorHistoricoDespesaFixa { get; set; }

        [Required]
        [Column("DH_REGISTRO_HISTORICO_DESPESA_FIXA")]
        public DateTime DataHoraRegistroHistoricoDespesaFixa { get; set; }

        [ForeignKey("IdDespesaFixa")]
        public virtual DespesaFixa DespesaFixa { get; set; }

    }
}
