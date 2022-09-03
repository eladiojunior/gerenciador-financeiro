using System.ComponentModel.DataAnnotations;

namespace gfin.webapi.Api.Models.Usuario
{
    public class LoginModel
    {
        [Required(ErrorMessage = "{0} não informada.")]
        [Display(Name = "Entidade de Controle")]
        public int IdEntidade { get; set; }

        [Required(ErrorMessage = "{0} não informado.")]
        [Display(Name = "E-mail do Usuário")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "{0} não informada.")]
        [DataType(DataType.Password)]
        [Display(Name="Senha do Usuário")]
        [StringLength(20, ErrorMessage = "{0} deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
        public string Senha { get; set; }
    }
}