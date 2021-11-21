using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public enum TipoPermissaoCompartilhamentoEnum
    {
        [Description("Podem Editar")]
        Edicao = 1,
        [Description("Podem Visualizar")]
		Visualizacao = 2
    }
}
