using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class LiquidaDespesaMensalModel
    {
        [Required(ErrorMessage = "Identificador da despesa não informado.")]
        public int IdDespesaMensal { get; set; }
        
        public string DescricaoNaturezaDespesaMensal { get; set; }
        public string DescricaoTipoFormaLiquidacao { get; set; }
        public string TextoDescricaoDespesaMensal { get; set; }
        public string DataVencimentoDespesa { get; set; }

        [Required(ErrorMessage = "Valor da despesa não informada.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public decimal ValorDespesa { get; set; }

        [Required(ErrorMessage = "Data de liquidação da despesa não informada.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Data de liquidação da despesa com formato inválido.")]
        public DateTime DataLiquidacaoDespesa { get; set; }
        
        public decimal? ValorDescontoLiquidacaoDespesa { get; set; }
        public decimal? ValorMultaJurosLiquidacaoDespesa { get; set; }
        public string ValorTotalLiquidacaoDespesa { get; set; }
        
        public string TextoObservacaoLiquidacaoDespesa { get; set; }

    }
}