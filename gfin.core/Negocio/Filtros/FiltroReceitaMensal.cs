using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Negocio.Filtros
{
    public class FiltroReceitaMensal
    {
        
        public bool HasTodas { get; set; }
        public bool HasRecebidas { get; set; }
        public bool HasAbertas { get; set; }
        public bool HasVencidas { get; set; }

        public DateTime? DataInicialFiltro { get; set; }

        public DateTime? DataFinalFiltro { get; set; }

    }
}
