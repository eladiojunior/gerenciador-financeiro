using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class LiquidaReceitaMensalModel
    {
        [Required(ErrorMessage = "Identificador da receita não informado.")]
        public int IdReceitaMensal { get; set; }
        
        public string DescricaoNaturezaReceitaMensal { get; set; }
        public string DescricaoTipoFormaRecebimento { get; set; }
        public string TextoDescricaoReceitaMensal { get; set; }
        public string DataRecebimentoReceita { get; set; }
        public string ValorReceita { get; set; }
        
        [Required(ErrorMessage = "Data de liquidação da receita não informada.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Data de liquidação da receita com formato inválido.")]
        public DateTime DataLiquidacaoReceita { get; set; }

        [Required(ErrorMessage = "Valor total de liquidação da receita não informada.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public decimal ValorTotalLiquidacaoReceita { get; set; }

    }
}