using GFin.Dados.Models;
using GFin.Web.Models.Filtros;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class ReceitaMensalModel
    {
        public int IdReceitaMensal { get; set; }
        
        public int IdNaturezaReceitaMensal { get; set; }
        
        public string TextoDescricaoReceitaMensal { get; set; }
        
        public short CodigoFormaRecebimentoReceitaMensal { get; set; }
        
        public bool IsReceitaRecebida { get; set; }

        [Required(ErrorMessage = "Data de recebimento da receita não informada.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Data de recebimento da receita com formato inválido.")]
        public DateTime DataRecebimentoReceita { get; set; }

        [Required(ErrorMessage = "Valor da receita não informado.")]
        public decimal ValorReceita { get; set; }
        
        public DropboxModel DropboxFormaRecebimento { get; set; }
        
        public DropboxModel DropboxNaturezaReceita { get; set; }

    }
}