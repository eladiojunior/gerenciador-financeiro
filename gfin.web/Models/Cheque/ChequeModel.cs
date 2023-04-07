using GFin.Dados.Models;
using GFin.Web.Models.Filtros;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class ChequeModel
    {
        public short CodigoSituacaoCheque { get; set; }
        public string DescicaoSituacaoCheque { get; set; }
        public DateTime DataHoraHistoricoCheque { get; set; }
        public string HistoricoCheque { get; set; }

    }
}