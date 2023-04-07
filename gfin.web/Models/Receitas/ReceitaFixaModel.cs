using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class ReceitaFixaModel
    {
        public int IdReceitaFixa { get; set; }
        public int IdNaturezaContaReceitaFixa { get; set; }
        public string TextoDescricaoReceitaFixa { get; set; }
        public short NumeroDiaRecebimentoReceitaFixa { get; set; }
        public decimal ValorReceitaFixa { get; set; }
        public DropboxModel DropboxDiaRecebimento { get; set; }
        public DropboxModel DropboxNaturezaReceita { get; set; }
    }
}