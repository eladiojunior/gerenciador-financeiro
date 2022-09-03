using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models.Helpers
{
    public class ConversorModel
    {
        /// <summary>
        /// Converte um valor (Mês/Ano) enviado em uma data valida, último dia do Mês e Ano informado.
        /// </summary>
        /// <param name="mesAno">Mês e ano (08/2016) informado para converter em uma data de validade válida.</param>
        /// <returns></returns>
        internal static DateTime ConverterDataValidade(string mesAno)
        {
            int mes = Int32.Parse(mesAno.Substring(0, 2));
            int ultimoDiaDoMes = Negocio.UtilNegocio.ObterDiasMes(mes);
            string strDataValidade = ultimoDiaDoMes + "/" + mesAno;
            return DateTime.Parse(strDataValidade);
        }
    }
}