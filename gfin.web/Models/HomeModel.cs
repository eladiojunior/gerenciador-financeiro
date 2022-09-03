using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class HomeModel
    {
        public List<DespesaMensal> ListaDespesa { get; set; }
        public List<ReceitaMensal> ListaReceita { get; set; }
    }
}