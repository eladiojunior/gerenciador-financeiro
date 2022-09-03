using System.ComponentModel.DataAnnotations;
using gfin.webapi.Negocio;

namespace gfin.webapi.Api.Models.Validation
{
    public class CnpjValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var cnpj = value.ToString();
            if (string.IsNullOrEmpty(cnpj)) return ValidationResult.Success;
            return !UtilNegocio.ValidarCNPJ(cnpj) ? 
                new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName)) : 
                ValidationResult.Success;
        }
    }
}