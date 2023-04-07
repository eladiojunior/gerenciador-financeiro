using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
{
    internal class CorrentistaDAO : GenericDAO<Correntista>
    {

        internal CorrentistaDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }

        /// <summary>
        /// Recupera a lista de nome de banco registrados no correntistas sem repetições (distinct).
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle financeiro do usuário.</param>
        /// <returns></returns>
        public List<string> ListarNomeBanco(int idEntidade)
        {
            return DbContextoGFin().Correntista.Where(c => c.IdEntidade == idEntidade).Select(c => c.NomeBanco).Distinct().ToList();
        }
    }
}
