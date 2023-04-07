using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalChequeModel
    {
        public int IdBancoAgenciaContaCorrente { get; set; }
        public string FormaPagamentoDespesaMensal { get; set; }
        public DateTime DataVencimentoDespesa { get; set; }
        public int NumeroCheque { get; set; }
        /// <summary>
        /// Identificador do Cheque para vinculação com a 
        /// despesa registrada.
        /// </summary>
        public int IdVinculoFormaLiquidacao { get; set; }

        public DropboxModel DropboxBancoAgenciaContaCorrente { get; set; }
    }
}