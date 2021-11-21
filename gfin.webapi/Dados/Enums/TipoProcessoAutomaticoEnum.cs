using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
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
