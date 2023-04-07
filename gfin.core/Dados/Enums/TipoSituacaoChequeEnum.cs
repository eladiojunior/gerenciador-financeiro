using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
{
    public enum TipoSituacaoChequeEnum
	{
        [Description("Não Informado")]
        NaoInformado = 0,
        [Description("Cheque Registrado")]
        ChequeRegistrado = 1,
        [Description("Cheque Emitido")]
		ChequeEmitido = 2,
        [Description("Cheque Compensado")]
        ChequeCompensado = 3,
        [Description("Cheque Cancelado")]
        ChequeCancelado = 4,
        [Description("Cheque Devolvido")]
        ChequeDevolvido = 5,
        [Description("Cheque Resgatado")]
        ChequeResgatado = 6
    }
}
