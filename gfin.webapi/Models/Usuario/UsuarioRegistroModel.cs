using System.ComponentModel.DataAnnotations;
using gfin.webapi.Api.Models.Validation;
using gfin.webapi.Dados.Enums;

namespace gfin.webapi.Api.Models.Usuario
{
    public class UsuarioRegistroModel
    {
        [Required(ErrorMessage = "{0} não informado.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "{0} não informado.")]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "{0} não informada.")]
        [Display(Name = "Senha de acesso")]
        [DataType(DataType.Password)]
        [SenhaValidation(ErrorMessage = "{0} inválida, informar no mínimo 6 caracteres (letras, números e caracteres especiais).")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "{0} não informada.")]
        [Display(Name="Confirmação de senha")]
        [DataType(DataType.Password)]
        [SenhaValidation(ErrorMessage = "{0} inválida, informar no mínimo 6 caracteres (letras, números e caracteres especiais).")]
        [Compare("Senha", ErrorMessage="Confirmação de senha não confere com a senha de acesso informada.")]
        public string ConfirmaSenha { get; set; }
        
        [Required(ErrorMessage = "{0} não informado.")]
        [Display(Name = "Tipo entidade controle")]
        [EnumsValidation(EnumType = typeof(TipoEntidadeControleEnum), ErrorMessage = "{0} inválida [Opções: {1}]")]
        public short CodigoTipoEntidadeControle { get; set; }

        [Display(Name = "Nome da empresa")]
        public string NomeEntidadeControle { get; set; }
        
        [CnpjValidation(ErrorMessage="{0} inválido.")]
        [Display(Name = "CNPJ")]
        public string CpfCnpjEntidadeControle { get; set; }
    }
}