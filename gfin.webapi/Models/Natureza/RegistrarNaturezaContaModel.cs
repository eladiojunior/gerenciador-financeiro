using System.ComponentModel.DataAnnotations;

namespace gfin.webapi.Api.Models
{
    public class RegistrarNaturezaContaModel
    {
        [Required]
        public short CodigoTipoLancamentoConta { get; set; }
        
        [Required]
        public string DescricaoNaturezaConta { get; set; }
    }
}