﻿using GFin.Web.Models.Helpers;
using GFin.Web.Models.Validacao;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class UsuarioRegistradoModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }

}