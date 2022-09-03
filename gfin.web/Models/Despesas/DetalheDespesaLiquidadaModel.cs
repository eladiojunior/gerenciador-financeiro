using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DetalheDespesaLiquidadaModel
    {
        public int IdDespesaMensal { get; set; }

        public string DescricaoNaturezaDespesaMensal { get; set; }
        public string DescricaoTipoFormaLiquidacao { get; set; }
        public string TextoDescricaoDespesaMensal { get; set; }
        public string DataVencimentoDespesa { get; set; }
        public string ValorDespesa { get; set; }
        public string DataLiquidacaoDespesa { get; set; }
        public string ValorDescontoLiquidacaoDespesa { get; set; }
        public string ValorMultaJurosLiquidacaoDespesa { get; set; }
        public string ValorTotalLiquidacaoDespesa { get; set; }
        public string TextoObservacaoLiquidacaoDespesa { get; set; }

    }
}