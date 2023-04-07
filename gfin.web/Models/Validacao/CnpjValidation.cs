using GFin.Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models.Validacao
{
    public class CnpjValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string cnpj = value.ToString();
                bool isValid = UtilNegocio.ValidarCNPJ(cnpj);
                if (!isValid)
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}