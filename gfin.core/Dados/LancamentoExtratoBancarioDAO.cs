using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
{
    internal class LancamentoExtratoBancarioDAO : GenericDAO<LancamentoExtratoBancario>
    {
        internal LancamentoExtratoBancarioDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
