using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalCartaoCreditoModel
    {
    
        public int IdBancoAgenciaContaCorrente { get; set; }

        public List<CartaoCredito> Cartoes { get; set; }

        public DropboxModel DropboxBancoAgenciaContaCorrente { get; set; }
    }
}