using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class NaturezaContaModel
    {
        public int IdNaturezaConta { get; set; }
        public short CodigoTipoLancamentoConta { get; set; }
        public string DescricaoNaturezaConta { get; set; }
        public DropboxModel DropboxTipoLancamentoConta { get; set; }
    }
}