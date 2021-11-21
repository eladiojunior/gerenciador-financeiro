using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
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
