using GFin.Dados.Erros;
using GFin.Dados.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace GFin.Dados
{
    internal class ChequeDAO : GenericDAO<Cheque>
    {
        internal ChequeDAO(GFinContext dbContexto) 
            : base(dbContexto)
        {
        }
        /// <summary>
        /// Recupera um cheque de uma determinada conta corrente e número do cheque.
        /// </summary>
        /// <param name="idContaCorrente">Identificador da conta corrente registrada.</param>
        /// <param name="numeroCheque">Numero do cheque a ser recuperado.</param>
        /// <returns></returns>
        internal Cheque ObterPorContaNumeroCheque(int idContaCorrente, int numeroCheque)
        {
            try
            {
                var query = DbContextoSet().AsQueryable();
                query = query.Where(c => c.IdContaCorrente == idContaCorrente && c.NumeroCheque == numeroCheque);
                return query.FirstOrDefault();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException erro)
            {
                throw new DAOException(erro);
            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao obter um cheque pelo identificador da conta corrente [{0}] e número do cheque [{1}].", idContaCorrente, numeroCheque), erro);
            }
        }
    }
}
