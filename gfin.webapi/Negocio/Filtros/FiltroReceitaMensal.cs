using System;

namespace gfin.webapi.Negocio.Filtros
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
