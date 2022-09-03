using GFin.Web.Models.Helpers;
using GFin.Web.Models.Validacao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class AlterarDadosUsuarioModel
    {
        public int IdUsuario { get; set; }
        public string FotoBase64 { get; set; }
        
        [Required]
        [Display(Name = "Nome completo")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int IdEntidade { get; set; }

        public short CodigoTipoEntidadeControle { get; set; }

        [Required]
        [Display(Name = "Nome da sua Casa ou Empresa")]
        public string NomeEntidadeControle { get; set; }

        [CnpjValidation(ErrorMessage = "{0} inválido.")]
        [Display(Name = "CNPJ da Empresa")]
        public string CpfCnpjEntidadeControle { get; set; }
    }

}