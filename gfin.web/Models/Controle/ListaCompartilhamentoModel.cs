using GFin.Dados.Models;
using GFin.Web.Models.Helpers;
using GFin.Web.Models.Validacao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class ListaCompartilhamentoModel
    {
        
        public List<ConviteCompartilhamento> Compartilhamentos { get; set; }
        
    }

}