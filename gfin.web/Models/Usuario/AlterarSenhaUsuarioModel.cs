using GFin.Web.Models.Helpers;
using GFin.Web.Models.Validacao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class AlterarSenhaUsuarioModel
    {
        public int IdUsuario { get; set; }
        public string FotoBase64 { get; set; }
        public string Nome { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Senha de acesso")]
        [DataType(DataType.Password)]
        [SenhaValidation(ErrorMessage = "{0} inválida, informar no mínimo 6 caracteres (letras, números e caracteres especiais).")]
        public string Senha { get; set; }

        [Required]
        [Display(Name = "Confirmação de Senha")]
        [DataType(DataType.Password)]
        [SenhaValidation(ErrorMessage = "{0} inválida, informar no mínimo 6 caracteres (letras, números e caracteres especiais).")]
        [Compare("Senha", ErrorMessage = "Confirmação de senha não confere com a senha de acesso informada.")]
        public string ConfirmaSenha { get; set; }

    }

}