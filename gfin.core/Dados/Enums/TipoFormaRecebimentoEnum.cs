using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
{
    public enum TipoFormaRecebimentoEnum
	{
        [Description("Não Informado")]
        NaoInformado = 0,
        
        [Description("Dinheiro")]
        Dinheiro = 1,

        [Description("Transferêcia")]
		Transferencia = 2,
        
        [Description("Cheque")]
		Cheche = 3,
        
        [Description("Crédito em Conta")]
        CreditoEmConta = 4,

    }
}
