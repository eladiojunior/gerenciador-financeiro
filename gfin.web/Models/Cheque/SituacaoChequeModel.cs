using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class SituacaoChequeModel
    {
        public int IdCheque { get; set; }
        public int NumeroCheque { get; set; }
        public short CodigoSituacaoCheque { get; set; }
        public string DescicaoSituacaoCheque { get; set; }
        public bool IsUtilizavel { get; set; }
    }
}