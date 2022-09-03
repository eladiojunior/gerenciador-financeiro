using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
{
    public enum TipoEntidadeControleEnum
	{
        [Description("Minha Casa")]
        Fisica = 1,
        [Description("Empresa")]
		Juridica = 2
    }
}
