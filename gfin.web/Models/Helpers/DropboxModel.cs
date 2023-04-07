using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Models.Helpers
{
    public class DropboxModel
    {
        private List<SelectListItem> itens = null;
        public List<SelectListItem> Itens { 
            get 
            {
                if (itens == null) 
                {
                    itens = new List<SelectListItem>();
                }
                return itens;
            }
            set 
            {
                itens = value;
            } 
        }
    }
}