namespace gfin.webapi.Api.Models
{
    public class EditarDespesaFixaModel
    {
        public int IdNaturezaContaDespesaFixa { get; set; }
        public string TextoDescricaoDespesaFixa { get; set; }
        public short NumeroDiaVencimentoDespesaFixa { get; set; }
        public decimal ValorDespesaFixa { get; set; }
        public short CodigoTipoFormaLiquidacao { get; set; }
    }
}