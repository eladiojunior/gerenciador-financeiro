using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
	public enum TipoFormaLiquidacaoEnum
	{
        [Description("Não Informado")]
        NaoInformado = 0,
        
        [Description("Dinheiro")]
        Dinheiro = 1,

        [Description("Cartão de Crédito/Débito")]
		CartaoCreditoDebito = 2,
        
        [Description("Cheque à Vista")]
		ChequeAVista = 3,
        
        [Description("Cheque Pré-Datado")]
		ChequePreDatado = 4,
        
        [Description("Boleto de Cobrança")]
		BoletoCobranca = 5,
        
        [Description("Débito em Conta")]
        DebitoEmConta = 6,

        [Description("Fatura")]
        FaturaMensal = 7

    }
}
