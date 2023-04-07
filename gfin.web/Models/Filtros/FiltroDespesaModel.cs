using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models.Filtros
{
    public class FiltroDespesaModel
    {

        //Filtro de pesquisa
        public DateTime DataInicialFiltro { get; set; }
        public bool IsFiltroTodas { get; set; }
        public bool IsFiltroPagas { get; set; }
        public bool IsFiltroAbertas { get; set; }
        public bool IsFiltroVencidas { get; set; }
        public DropboxModel DropboxFiltroMesAno { get; set; }
        
        //Lista de Despesas
        public List<DespesaMensal> DespesasMensais { get; set; }

        //Totalizadores
        public decimal ValorTotalDespesaPagas { get; set; }
        public decimal ValorTotalDespesaAbertas { get; set; }
        public decimal ValorTotalDespesaVencidas { get; set; }
        public decimal ValorTotalDespesa { get; set; }


    }
}