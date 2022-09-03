using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models.Filtros
{
    public class FiltroChequeModel
    {
        public FiltroChequeModel()
        {
            ListaCheques = new List<Cheque>();
        }
        public int IdBancoAgenciaContaCorrente { get; set; }
        public short CodigoSituacaoCheque { get; set; }
        public List<Cheque> ListaCheques { get; set; }
        public DropboxModel DropboxTipoSituacaoCheque { get; set; }
    }
}