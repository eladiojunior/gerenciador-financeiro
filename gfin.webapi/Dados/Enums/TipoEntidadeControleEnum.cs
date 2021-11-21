using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public enum TipoEntidadeControleEnum
	{
        [Description("Minha Casa")]
        Fisica = 1,
        [Description("Empresa")]
		Juridica = 2
    }
}
