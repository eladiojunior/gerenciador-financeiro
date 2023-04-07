using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaFixaModel
    {
        public int IdDespesaFixa { get; set; }
        public int IdNaturezaContaDespesaFixa { get; set; }
        public string TextoDescricaoDespesaFixa { get; set; }
        public short NumeroDiaVencimentoDespesaFixa { get; set; }
        public decimal ValorDespesaFixa { get; set; }
        public short CodigoTipoFormaLiquidacao { get; set; }
        public DropboxModel DropboxFormaLiquidacao { get; set; }
        public DropboxModel DropboxDiaVencimento { get; set; }
        public DropboxModel DropboxNaturezaDespesa { get; set; }
    }
}