using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models.Filtros
{
    public class FiltroReceitaModel
    {

        //Filtro de pesquisa
        public DateTime DataInicialFiltro { get; set; }
        public bool IsFiltroTodas { get; set; }
        public bool IsFiltroRecebidas { get; set; }
        public bool IsFiltroAbertas { get; set; }
        public bool IsFiltroVencidas { get; set; }
        public DropboxModel DropboxFiltroMesAno { get; set; }
        
        //Lista de Receitas
        public List<ReceitaMensal> ReceitasMensais { get; set; }

        //Totalizadores
        public decimal ValorTotalReceitaRecebidas { get; set; }
        public decimal ValorTotalReceitaAbertas { get; set; }
        public decimal ValorTotalReceitaVencidas { get; set; }
        public decimal ValorTotalReceita { get; set; }


    }
}