using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class DespesaMensalParcelamentoCartaoCreditoModel
    {

        public int IdBancoAgenciaContaCorrente { get; set; }
        public List<CartaoCredito> Cartoes { get; set; }
        public DropboxModel DropboxBancoAgenciaContaCorrente { get; set; }
        public string FormaPagamentoDespesaMensal { get; set; }
        public int QtdParcelasDespesa { get; set; }
        public List<DespesaMensalParcelaModel> ParcelasDespesa { get; set; }
    }
}