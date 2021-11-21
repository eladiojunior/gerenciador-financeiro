using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public enum TipoPerfilAcessoUsuarioEnum
	{
        [Description("Administrador")]
        Administrador = 9,
        [Description("Responsável")]
        Responsavel = 1,
        [Description("Convidado Editor")]
		ConvidadoEditor = 2,
        [Description("Convidado Visualizador")]
        ConvidadoViusualizador = 3
    }
}
