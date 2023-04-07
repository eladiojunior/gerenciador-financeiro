using GFin.Dados;
using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
{
    internal class ProcessoAutomaticoDAO : GenericDAO<ProcessoAutomatico>
    {
        internal ProcessoAutomaticoDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
    }
}
