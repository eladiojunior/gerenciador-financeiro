using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
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
