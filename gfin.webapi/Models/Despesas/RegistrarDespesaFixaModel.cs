using System.ComponentModel.DataAnnotations;

namespace gfin.webapi.Api.Models
{
    public class RegistrarDespesaFixaModel
    {
        [Required]
        public int IdNaturezaContaDespesaFixa { get; set; }
        
        [Required]
        public string TextoDescricaoDespesaFixa { get; set; }
        
        [Required]
        public short NumeroDiaVencimentoDespesaFixa { get; set; }
        
        [Required]
        public decimal ValorDespesaFixa { get; set; }
        [Required]
        public short CodigoTipoFormaLiquidacao { get; set; }
    }
}