using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
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
