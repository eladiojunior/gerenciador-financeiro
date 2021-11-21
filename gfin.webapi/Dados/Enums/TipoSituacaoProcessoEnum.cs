using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
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
