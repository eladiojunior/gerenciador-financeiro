using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
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
