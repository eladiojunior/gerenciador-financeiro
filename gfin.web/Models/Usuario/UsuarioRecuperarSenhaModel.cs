using GFin.Web.Models.Helpers;
using GFin.Web.Models.Validacao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class UsuarioRecuperarSenhaModel
    {
        public int IdUsuario { get; set; }

        [Required]
        [Display(Name = "E-mail registrado")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Código recuperação")]
        public string CodigoSeguranca { get; set; }
    }
}