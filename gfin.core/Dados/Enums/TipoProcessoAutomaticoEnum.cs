using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Enums
{
    public enum TipoProcessoAutomaticoEnum
    {
        [Description("Não Informado")]
        NaoInformado = 0,
        [Description("Contas Fixas Mensal")]
        ContasFixasMensal = 1,
        [Description("Importação de arquivo de Conta Corrente")]
        ImportacaoArquivoContaCorrente = 2
    }
}
