using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Service
{
    static class Program
    {
        /// <summary>
        /// Implementação de windows service para processo que verificará as 
        /// contas (despesas e receitas) fixa que devem ser criadas como mensal.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new VerificarContasService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
