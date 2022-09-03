using System;
using System.ComponentModel.DataAnnotations;
using gfin.webapi.Dados.Enums;

namespace gfin.webapi.Api.Models.Validation
{
    public class EnumsValidation : ValidationAttribute
    {
        public Type EnumType { get; set; }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var isValid = false;
            if (short.TryParse(value.ToString(), out var codigoTipoEnum))
            {//Verificar se o código possui no Enum;
                isValid = UtilEnum.IsEnumValido(EnumType, codigoTipoEnum);
            }

            if (!isValid)
            {
                var displayName = string.IsNullOrEmpty(validationContext.DisplayName)?"Enum":validationContext.DisplayName;
                var listaOpcoes = string.Join(", ", UtilEnum.ListaEnums(EnumType));;
                string mensagem = string.Format(ErrorMessage, displayName, listaOpcoes);
                return new ValidationResult(mensagem);
            }

            return ValidationResult.Success;
        }
    }
}