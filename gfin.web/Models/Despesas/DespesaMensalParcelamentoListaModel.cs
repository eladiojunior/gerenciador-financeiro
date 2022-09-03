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
    public class DespesaMensalParcelamentoListaModel
    {
        public int IdDespesaMensal { get; set; }
        
        public string DescricaoTipoFormaLiquidacao { get; set; }
        
        public short? QtdParcelasDespesa { get; set; }
        public List<DespesaMensalParcelamentoListaParcelaModel> ParcelasDespesa { get; set; }

    }
    public class DespesaMensalParcelamentoListaParcelaModel
    {
        public int IdDespesaMensal { get; set; }
        public string DescricaoNaturezaDespesaMensal { get; set; }
        public string NumeroParcelaDespesa { get; set; }
        public string DescricaoDespesaMensal { get; set; }
        public DateTime DataVencimentoDespesa { get; set; }
        public decimal ValorDespesa { get; set; }
        public bool IsDespesaLiquidada { get; set; }
        public DateTime? DataLiquidacaoDespesa { get; set; }
    }
}