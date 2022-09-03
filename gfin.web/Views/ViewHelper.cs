using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace GFin.Web.Views
{
    public class ViewHelper
    {
        private static string _version = null;
        /// <summary>
        /// Recupera a versão da aplicação no Assembly.
        /// </summary>
        /// <returns></returns>
        public static string ObterVersaoAplicacao()
        {
            if (string.IsNullOrEmpty(_version))
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                _version = fvi.FileVersion;
            }
            return _version;
        }
    }
}