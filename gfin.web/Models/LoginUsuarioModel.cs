using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class LoginUsuarioModel
    {

        [Required]
        [Display(Name = "Entidade de Controle")]
        public int IdEntidade { get; set; }

        [Required]
        [Display(Name = "E-mail do Usuário")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name="Senha do Usuário")]
        [StringLength(20, ErrorMessage = "{0} deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
        public string Senha { get; set; }

        public bool ManterConectado { get; set; }

        public DropboxModel DropboxEntidades { get; set; }

        public string RetornoUrl { get; set; }
    }
}