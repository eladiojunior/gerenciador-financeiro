using GFin.Core.Negocio.DTOs;
using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models.Filtros
{
    public class FiltroContasModel
    {

        //Filtro de pesquisa
        public DateTime DataInicialFiltro { get; set; }
        public bool IsExibirDespesas { get; set; }
        public bool IsExibirReceitas { get; set; }
        public bool IsExibirLiquidadas { get; set; }
        public bool IsExibirContasAbertasMesesAnteriores { get; set; }
        public DropboxModel DropboxFiltroMesAno { get; set; }
        
        //Lista de Despesas e Receitas
        public List<ContasMensalDTO> ContasMensais { get; set; }

        //Totalizadores de Despesas
        public decimal ValorTotalDespesasLiquidadas { get; set; }
        public decimal ValorTotalDespesasAbertas { get; set; }
        public decimal ValorTotalDespesasVencidas { get; set; }
        public decimal ValorTotalDespesas { get; set; }
        
        //Totalizadores de Receitas
        public decimal ValorTotalReceitasLiquidadas { get; set; }
        public decimal ValorTotalReceitasAbertas { get; set; }
        public decimal ValorTotalReceitasVencidas { get; set; }
        public decimal ValorTotalReceitas { get; set; }


    }
}