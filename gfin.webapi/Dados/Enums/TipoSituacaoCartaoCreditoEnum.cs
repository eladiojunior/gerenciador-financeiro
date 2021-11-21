using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public enum TipoSituacaoCartaoCreditoEnum
    {
        [Description("Selecione")]
        NaoInformado = 0,
        [Description("Ativo")]
        Ativo = 1,
        [Description("Bloqueado")]
		Bloqueado = 2,
        [Description("Cancelado")]
        Cancelado = 3
    }
}
