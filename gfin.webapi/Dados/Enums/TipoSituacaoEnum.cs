using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public enum TipoSituacaoEnum
	{
        [Description("Selecione")]
        NaoInformado = 0,
        [Description("Ativo")]
        Ativo = 1,
        [Description("Inativo")]
		Inativo = 2
    }
}
