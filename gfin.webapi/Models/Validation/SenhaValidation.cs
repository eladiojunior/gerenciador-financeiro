using System.ComponentModel.DataAnnotations;
using gfin.webapi.Negocio;

namespace gfin.webapi.Api.Models.Validation
{
    public class SenhaValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string senha = value.ToString();
                bool isValid = UtilNegocio.ValidarSenha(senha);
                if (!isValid)
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}