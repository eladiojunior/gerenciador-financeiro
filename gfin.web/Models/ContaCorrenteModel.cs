using GFin.Dados.Models;
using System.Collections.Generic;

namespace GFin.Web.Models
{
    public class ContaCorrenteModel
    {
        public int IdContaCorrente { get; set; }
        public int NumeroBanco { get; set; }
        public string NomeBanco { get; set; }
        public string NumeroAgencia { get; set; }
        public string NumeroContaCorrente { get; set; }
        public string NomeTitular { get; set; }
        public decimal ValorLimite { get; set; }
        public bool IsContaCorrenteAtiva { get; set; }
    }
}