using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
{
    public enum TipoSituacaoProcessoEnum
    {
        [Description("Criado")]
        Criado = 1,
        [Description("Em Processamento")]
        EmProcessamento = 2,
        [Description("Processado")]
        Processado = 3,
        [Description("Cancelado")]
        Cancelado = 4,
        [Description("Erro")]
        Erro = 9
    }
}
