using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class NovaNaturezaContaModel
    {
        public short CodigoTipoLancamentoConta { get; set; }
        public string TipoLancamentoConta { get; set; }
        public string DescricaoNaturezaConta { get; set; }
        public string IdDropdownRetorno { get; internal set; }
    }
}