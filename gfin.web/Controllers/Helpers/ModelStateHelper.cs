using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers.Helpers
{
    public static class ModelStateHelper
    {
        /// <summary>
        /// Recupera a lista de erros no componente ModelState quando existir.
        /// </summary>
        /// <param name="modelState">ModelState atual na session.</param>
        /// <returns></returns>
        public static IEnumerable Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                List<string> resultErros = new List<string>();
                var errors = modelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in errors)
                {
                    resultErros.Add(error);
                }
                return resultErros;
            }
            return null;
        }
    }
}
