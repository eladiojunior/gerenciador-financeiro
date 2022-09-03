using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Core.Negocio.DTOs
{
    public class ContasMensalDTO
    {
        public int IdConta { get; set; }
        public string DescricaoNaturezaConta { get; set; }
        public string DescricaoConta { get; set; }
        public DateTime DataConta { get; set; }
        public string DescricaoTipoFormaLiquidacao { get; set; }
        public bool IsContaLiquidada { get; set; }
        public DateTime? DataLiquidacaoConta { get; set; }
        public bool IsContaVencida { get; set; }
        public int QtdDiasVencimento { get; set; }
        public decimal ValorConta { get; set; }
        /// <summary>
        /// Código de identificação do tipo de conta (D=Despesa ou R=Receita)
        /// </summary>
        public string CodigoTipoConta { get; set; }
    }
}
