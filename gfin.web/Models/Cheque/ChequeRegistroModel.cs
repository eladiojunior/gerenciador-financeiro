using GFin.Dados.Models;
using GFin.Web.Models.Filtros;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class ChequeRegistroModel
    {
        public int IdBancoAgenciaContaCorrente { get; set; }
        public string BancoAgenciaContaCorrente { get; set; }
        public int? NumeroChequeInicial { get; set; }
        public int? NumeroChequeFinal { get; set; }
        
        public int QtdChequeRegistro { get; set; }
        public List<SituacaoChequeModel> ChequesRegistro { get; set; }
        public FiltroChequeModel FiltroCheque { get; set; }
        public DropboxModel DropboxBancoAgenciaContaCorrente { get; set; }

    }
}