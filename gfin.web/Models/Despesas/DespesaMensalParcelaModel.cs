using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalParcelaModel
    {
        //Identificador de vinculação com a despesa do Boleto, 
        //Cartão de Crédito, Débito em Conta ou Cheque À Vista 
        //ou Pre-Datado;
        public int? IdVinculoFormaLiquidacao { get; set; }
        public int? NumeroParcela { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorParcela { get; set; }

    }
}