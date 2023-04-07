using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class CorrentistaModel
    {
        public int IdCorrentista { get; set; }
        public string NomeBanco { get; set; }
        public string NumeroAgencia { get; set; }
        public string NumeroContaCorrente { get; set; }
        public string NomeCorrentista { get; set; }
        public string Observacao { get; set; }
        public DropboxModel DropboxNomeBanco { get; set; }
    }
}