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
    public class DespesaMensalModel
    {
        public int IdDespesaMensal { get; set; }
        
        public int IdNaturezaDespesaMensal { get; set; }
        
        public string TextoDescricaoDespesaMensal { get; set; }
        
        public short CodigoFormaPagamentoDespesaMensal { get; set; }
        
        public bool IsDespesaLiquidada { get; set; }

        //Identificado que a despesa será parcelada essas informações
        //serão preenchidas pela interface.
        public int IdBancoAgenciaContaCorrente { get; set; }
        public bool IsDespesaMensalParcelada { get; set; }
        public short? QtdParcelasDespesa { get; set; }
        public List<DespesaMensalParcelaModel> ParcelasDespesa { get; set; }

        [Required(ErrorMessage = "Data de venciamento da despesa não informada.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Data de venciamento da despesa com formato inválido.")]
        public DateTime DataVencimentoDespesa { get; set; }

        [Required(ErrorMessage = "Valor da despesa não informado.")]
        public decimal ValorDespesa { get; set; }
        
        public DateTime? DataLiquidacaoDespesa { get; set; }

        public decimal? ValorDescontoLiquidacaoDespesa { get; set; }

        public decimal? ValorMultaJurosLiquidacaoDespesa { get; set; }

        public string TextoObservacaoLiquidacaoDespesa { get; set; }

        public decimal? ValorTotalLiquidacaoDespesa { get; set; }

        //Identificador de vinculação com a despesa do Boleto, 
        //Cartão de Crédito, Débito em Conta ou Cheque À Vista;
        public int? IdVinculoFormaLiquidacao { get; set; }

        public FiltroDespesaModel FiltroDespesas { get; set; }
        
        public DropboxModel DropboxFormaLiquidacao { get; set; }
        
        public DropboxModel DropboxNaturezaDespesa { get; set; }

    }
}