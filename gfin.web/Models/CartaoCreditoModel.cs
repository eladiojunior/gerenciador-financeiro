using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System.Collections.Generic;

namespace GFin.Web.Models
{
    public class CartaoCreditoModel
    {
        public int IdCartaoCredito { get; set; }
        public int IdBancoAgenciaContaCorrente { get; set; }
        public string NumeroCartao { get; set; }
        public string NomeCartao { get; set; }
        public string MesAnoValidadeCartao { get; set; }
        public bool HasCartaoCredito { get; set; }
        public bool HasCartaoDebito { get; set; }
        public bool HasCartaoPrePago { get; set; }
        public decimal ValorLimiteCartao { get; set; }
        public short DiaVencimentoCartaoCredito { get; set; }
        public string NomeProprietarioCartao { get; set; }
        public short SituacaoCartao { get; set; }
        public DropboxModel DropboxBancoAgenciaContaCorrente { get; set; }
        public DropboxModel DropboxTipoSituacaoCartao { get; set; }
    }
}