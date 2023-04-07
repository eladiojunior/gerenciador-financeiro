using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DetalheReceitaLiquidadaModel
    {
        public int IdReceitaMensal { get; set; }
        public string DescricaoNaturezaReceitaMensal { get; set; }
        public string DescricaoTipoFormaRecebimento { get; set; }
        public string TextoDescricaoReceitaMensal { get; set; }
        public string DataRecebimentoReceita { get; set; }
        public string ValorReceita { get; set; }
        public string DataLiquidacaoReceita { get; set; }
        public string ValorTotalLiquidacaoReceita { get; set; }

    }
}