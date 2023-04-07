﻿using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
{
    internal class ExtratoBancarioDAO : GenericDAO<ExtratoBancarioConta>
    {
        internal ExtratoBancarioDAO(GFinContext dbContexto) : base(dbContexto) { }
    }
}
