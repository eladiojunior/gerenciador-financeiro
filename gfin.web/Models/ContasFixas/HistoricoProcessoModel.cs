using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class HistoricoProcessoModel
    {
        public string DataHoraProcesso { get; set; }
        public string NomeProcesso { get; set; }
        public List<GFin.Dados.Models.HistoricoSituacaoProcesso> ListaHistoricoProcesso { get; set; }
    }
}