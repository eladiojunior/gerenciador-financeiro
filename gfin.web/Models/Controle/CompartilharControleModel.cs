using GFin.Web.Models.Helpers;
using GFin.Web.Models.Validacao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class CompartilharControleModel
    {
        
        [Required]
        [Display(Name = "E-mails dos Convidados")]
        public string Emails { get; set; }
        
        [Required]
        [Display(Name = "Permissão")]
        public short CodigoPermissao { get; set; }

        [Display(Name = "Mensagem do Convite")]
        [MaxLength(500, ErrorMessage="{0} deve ter no máximo {1} carecteres.")]
        public string Mensagem { get; set; }
        
        public DropboxModel DropboxPermissoesCompartilhamento { get; set; }
    }

}