using System;
using System.Collections.Generic;

namespace gfin.webapi.Dados.Models
{
    /// <summary>
    /// Objeto que representa uma coleção de despesas, parcelamento.
    /// Não deve ser mantido em banco, será utilizado como TO da view para negócio.
    /// </summary>
    public class DespesaMensalParcelada
    {
        public int IdNaturezaContaDespesa { get; set; }
        public string TextoDescricaoDespesa { get; set; }
        public List<ParcelaDespesa> ParcelamentoDespesa { get; set; }
        public short CodigoTipoFormaLiquidacao { get; set; }
    }
    public class ParcelaDespesa
    {
        public int NumeroParcela { get; set; }
        public int CodigoVinculoFormaLiquidacao { get; set; }
        public DateTime DataVencimentoParcela { get; set; }
        public decimal ValorParcela { get; set; }
        public bool IsLiquidada { get; set; }
    }

}
