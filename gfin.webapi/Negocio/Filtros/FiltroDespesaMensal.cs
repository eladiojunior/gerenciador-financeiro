using System;

namespace gfin.webapi.Negocio.Filtros
{
    public class FiltroDespesaMensal
    {
        
        public bool HasTodas { get; set; }
        public bool HasPagas { get; set; }
        public bool HasAbertas { get; set; }
        public bool HasVencidas { get; set; }

        public DateTime? DataInicialVencimento { get; set; }

        public DateTime? DataFinalVencimento { get; set; }

    }
}
