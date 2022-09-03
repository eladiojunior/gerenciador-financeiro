using GFin.Dados.Models;
using GFin.Web.Models.Filtros;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalEdicaoModel
    {
        public int IdDespesaMensal { get; set; }
        
        public int IdNaturezaDespesaMensal { get; set; }
        
        public string TextoDescricaoDespesaMensal { get; set; }
        
        public short CodigoFormaPagamentoDespesaMensal { get; set; }
        
        public string DescricaoTipoFormaLiquidacao { get; set; }

        public bool IsDespesaMensalParcelada { get; set; }
        public string NumeroParcelaDespesa { get; set; }
        public short? QtdParcelasDespesa { get; set; }

        [Required(ErrorMessage = "Data de venciamento da despesa não informada.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Data de venciamento da despesa com formato inválido.")]
        public DateTime DataVencimentoDespesa { get; set; }

        [Required(ErrorMessage = "Valor da despesa não informado.")]
        public decimal ValorDespesa { get; set; }
        
        public DropboxModel DropboxNaturezaDespesa { get; set; }

    }
}