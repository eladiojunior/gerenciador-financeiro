using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalParcelamentoDinheiroModel
    {
    
        public int IdNaturezaDespesaMensal { get; set; }

        public string TextoDescricaoDespesaMensal { get; set; }

        public short CodigoFormaPagamentoDespesaMensal { get; set; }

        public string FormaPagamentoDespesaMensal { get; set; }

        public int QtdParcelasDespesa { get; set; }

        public List<DespesaMensalParcelaModel> ParcelasDespesa { get; set; }
    }
}