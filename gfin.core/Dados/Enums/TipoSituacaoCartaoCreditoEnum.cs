using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
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
