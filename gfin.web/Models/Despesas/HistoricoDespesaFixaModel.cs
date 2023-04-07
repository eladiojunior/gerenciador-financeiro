using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class HistoricoDespesaFixaModel
    {
        public int IdDespesaFixa { get; set; }
        public string TextoDescricaoDespesaFixa { get; set; }
        
        public List<HistoricoDespesaFixa> HistoricoDespesasFixas { get; set; }

    }
}