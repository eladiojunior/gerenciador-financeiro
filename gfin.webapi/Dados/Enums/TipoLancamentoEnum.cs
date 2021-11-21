using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public enum TipoLancamentoEnum
	{
        [Description("Não Informado")]
        NaoInformado = 0,
        [Description("Despesa")]
        Despesa = 1,
        [Description("Receita")]
		Receita = 2
    }
}
