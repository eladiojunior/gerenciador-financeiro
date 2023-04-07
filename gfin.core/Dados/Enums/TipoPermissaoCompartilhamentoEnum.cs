using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
{
    public enum TipoPermissaoCompartilhamentoEnum
    {
        [Description("Podem Editar")]
        Edicao = 1,
        [Description("Podem Visualizar")]
		Visualizacao = 2
    }
}
