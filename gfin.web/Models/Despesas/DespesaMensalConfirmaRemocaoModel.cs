using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalConfirmaRemocaoModel
    {
        public int IdDespesaMensal { get; set; }
        public string DescricaoNaturezaDespesaMensal { get; set; }
        public string DescricaoTipoFormaLiquidacao { get; set; }
        public string TextoDescricaoDespesaMensal { get; set; }
        public string DataVencimentoDespesa { get; set; }
        public string ValorDespesa { get; set; }
        public bool HasDespesaParcelada { get; set; }
        public int QtdParcelas { get; set; }
        public bool HasRemoverParcelamento { get; set; }
    }
}