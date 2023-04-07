using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalDebitoContaModel
    {
    
        public int IdBancoAgenciaContaCorrente { get; set; }
        public string FormaPagamentoDespesaMensal { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorDespesa { get; set; }
        public DropboxModel DropboxBancoAgenciaContaCorrente { get; set; }
    }
}