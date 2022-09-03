using GFin.Dados.Models;
using GFin.Web.Models.Filtros;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class HistoricoChequeModel
    {
        public int IdBancoAgenciaContaCorrente { get; internal set; }
        public string BancoAgenciaContaCorrente { get; set; }
        public int NumeroCheque { get; set; }
        public List<ChequeModel> ListaHistoricoCheque { get; set; }
        public string DescricaoSituacaoAtualCheque { get; internal set; }
    }
}